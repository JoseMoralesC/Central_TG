package java_proveedor;

import java_proveedor.src.config.Config_Loader;
import java_proveedor.src.database.ConexionSQL;
import java_proveedor.src.sockets.ManejoCliente;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.sql.Connection;

public class Main {
    public static void main(String[] args) {
        System.out.println("=========================================");
        System.out.println("   INICIANDO COMPONENTE PROVEEDOR (JAVA) ");
        System.out.println("=========================================");

        // 1. Verificación Inicial de Conexión a SQL Server (Local / Tailscale)
        try (Connection conn = ConexionSQL.getConexion()) {
            if (conn != null && !conn.isClosed()) {
                System.out.println("[BD] OK - Conexión exitosa con SQL Server.");
            }
        } catch (Exception e) {
            System.err.println("[BD] CRÍTICO - No se pudo conectar a la base de datos.");
            System.err.println("[BD] Detalle: " + e.getMessage());
            System.err.println("[BD] El servidor continuará, pero las transacciones fallarán.");
        }

        // 2. Leer el puerto desde el config.properties (Fase 1)
        String puertoStr = Config_Loader.get("socket.puerto");
        int puerto = (puertoStr != null) ? Integer.parseInt(puertoStr) : 6000;

        // 3. Abrir el ServerSocket para escuchar a Python (Fase 2 del Roadmap)
        try (ServerSocket servidorSocket = new ServerSocket(puerto)) {
            System.out.println("[Server] OK - Escuchando en el puerto: " + puerto);
            System.out.println("[Server] Esperando conexiones de Python Identificador...");
            System.out.println("=========================================");

            // Bucle infinito: Mantiene el programa vivo aceptando clientes uno por uno
            while (true) {
                // El programa se "pausa" aquí hasta que entre una petición por red
                Socket clienteSocket = servidorSocket.accept();
                System.out.println("[Server] ¡Conexión recibida desde " + clienteSocket.getInetAddress() + "!");

                // Creamos el hilo con tu clase ManejoCliente pasándole el socket abierto
                ManejoCliente hiloCliente = new ManejoCliente(clienteSocket);
                
                // .start() arranca el método run() de ManejoCliente en un hilo independiente.
                // Esto permite que si entran múltiples solicitudes a la vez, el servidor no se pegue.
                hiloCliente.start();
            }

        } catch (IOException e) {
            System.err.println("[Server] CRÍTICO - Error en el Servidor de Sockets: " + e.getMessage());
            e.printStackTrace();
        }
    }
}