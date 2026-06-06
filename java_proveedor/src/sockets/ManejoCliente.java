package java_proveedor.src.sockets;

//Imports
import java.net.Socket;
import java.io.*;
import java.nio.charset.StandardCharsets;

public class ManejoCliente extends Thread
{
    //Atributos
    private Socket clienteSocket;
    //Clases instanciadas desde la carpeta de servicios

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

            if(solicitud_llamada != null)
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
                case "registrar_producto":
                    //Lógica para registrar producto
                    break;

                case "actualizar_inventario":
                    //Lógica para actualizar inventario
                    break;

                case "consultar_ventas":
                    //Lógica para consultar ventas
                    break;
                    
                default:
                    writer.println("Acción no reconocida: " + accion);
            }

        }

        catch (Exception e)
        {
            e.printStackTrace();
        }

    }

    public String leerCampo(String solicitud_llamada, String campo)
    {

        try
        {
            int posClave = solicitud_llamada.indexOf("\"" + campo + "\":");

            if (posClave != -1) return "";
            
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
