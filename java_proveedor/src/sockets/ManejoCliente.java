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
    ConsultaSaldo consultaSaldo = new ConsultaSaldo();
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
                    // Extraemos los campos que Python enviará en la solicitud de inicio
                    String origen = leerCampo(solicitud_llamada, "origen");
                    String destinoTipo = leerCampo(solicitud_llamada, "tipo_destino"); // Ej: LOCAL
                    
                    // Ejecutamos la lógica de negocio de las Fases 4 y 5
                    String respuestaVerificacion = verificarSaldo.procesarVerificacionLlamada(origen, destinoTipo);
                    
                    // Respondemos el JSON estructurado directo a la red de Tailscale
                    writer.println(respuestaVerificacion);
                    break;

                case "FINALIZAR_LLAMADA":
                    // Aquí procesarás la duración de la llamada, calcularás la tasa (HU3)
                    // y actualizarás la base de datos local de SQL Server.
                    writer.println("{\"status\": \"OK\", \"mensaje\": \"Llamada procesada y facturada\"}");
                    break;

                case "CONSULTAR_SALDO":
                    // Extraemos el número de teléfono para la consulta informativa
                    String numero = leerCampo(solicitud_llamada, "numero");
                    
                    // Usamos tu variable oficial 'consultaSaldo'
                    // Para una consulta de saldo pura (HU1), asumimos una tarifa por defecto mandando "LOCAL"
                    String resultadoSaldo = consultaSaldo.procesarConsulta(numero);
                    
                    if ("ERROR".equals(resultadoSaldo)) {
                        writer.println("{\"status\": \"ERROR\", \"mensaje\": \"Línea inválida\"}");
                    } else {
                        writer.println("{\"status\": \"OK\", \"saldo\": \"" + resultadoSaldo + "\"}");
                    }
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
