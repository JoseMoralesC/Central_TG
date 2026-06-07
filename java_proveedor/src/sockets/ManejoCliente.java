package java_proveedor.src.sockets;

//Imports de las clases de servicios
import java_proveedor.src.services.CalculoTarifa;
import java_proveedor.src.services.ConsultaSaldo;
import java_proveedor.src.services.RegistrarMovimiento;
import java_proveedor.src.services.VerificarSaldo;

//Imports
import java.net.Socket;
import java.io.*;
import java.nio.charset.StandardCharsets;

public class ManejoCliente extends Thread
{
    //Atributos
    private Socket clienteSocket;

    //Clases instanciadas desde la carpeta de servicios
    ConsultaSaldo calculoSaldo = new ConsultaSaldo();
    CalculoTarifa calculoTarifa = new CalculoTarifa();
    RegistrarMovimiento registrarMovimiento = new RegistrarMovimiento();
    VerificarSaldo verificarSaldo = new VerificarSaldo();


    public ManejoCliente(Socket clienteSocket)
    {
        this.clienteSocket = clienteSocket;
    }

    @Override
    public void run()
    {
        //Esto lo sugirio la IA para manejar la comunicacion con el cliente en el try
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(clienteSocket.getInputStream(), StandardCharsets.UTF_8));
            PrintWriter writer = new PrintWriter(new OutputStreamWriter(clienteSocket.getOutputStream(), StandardCharsets.UTF_8), true))
        {
            String solicitud_llamada = reader.readLine();

            if(solicitud_llamada == null || solicitud_llamada.trim().isEmpty())
            {
                writer.println("Llamada recibida: " + solicitud_llamada);
                return;
            }

            String accion = leerCampo(solicitud_llamada, "accion");

            if (accion.isEmpty())
            {
                return;
            }
            
            switch (accion)
            {
                case "INICIAR_LLAMADA":
                    // Aquí llamarás a tus servicios/DAOs para verificar si la línea está activa (HU1)
                    // y si cuenta con saldo suficiente (HU2).
                    writer.println("{\"status\": \"OK\", \"mensaje\": \"Llamada autorizada\"}");
                    break;

                case "FINALIZAR_LLAMADA":
                    // Aquí procesarás la duración de la llamada, calcularás la tasa (HU3)
                    // y actualizarás la base de datos local de SQL Server.
                    writer.println("{\"status\": \"OK\", \"mensaje\": \"Llamada procesada y facturada\"}");
                    break;

                case "CONSULTAR_SALDO":
                    break;
                    
                default:
                    writer.println("{\"error\": \"Acción no reconocida: " + accion + "\"}");
            }

        }

        catch (Exception e)
        {
            e.printStackTrace();
        }

        finally 
        {
            // Asegurar que el socket se cierre al terminar la comunicación de la trama
            try {
                if (clienteSocket != null && !clienteSocket.isClosed()) {
                    clienteSocket.close();
                }
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }

    }

    //Se lee el JSON en forma nativa, sin usar librerias externas.
    public String leerCampo(String solicitud_llamada, String campo)
    {

        try
        {
            int posClave = solicitud_llamada.indexOf("\"" + campo + "\":");
            
            //Si no lo encuentra, retorna vacío
            if (posClave == -1) return "";
            
            int posDosPuntos = solicitud_llamada.indexOf(":", posClave);
            int posAbreComillas = solicitud_llamada.indexOf("\"", posDosPuntos);
            int posCierraComillas = solicitud_llamada.indexOf("\"", posAbreComillas + 1);

            return solicitud_llamada.substring(posAbreComillas + 1, posCierraComillas);
        }

        catch (Exception e)
        {
            return "";
        }
    }
     
}
