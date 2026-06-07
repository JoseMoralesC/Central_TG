//Package
package java_proveedor.src.sockets;

//Imports
import java.net.ServerSocket;
import java.net.Socket;



//Clase Socket
public class SocketTCP
{

    private static final int puerto = 6000;

    public static void main(String[] args)
    {
        //Cada que entre un nuevo cliente, va a ser atendido por un nuevo hilo
        try (ServerSocket serverSocket = new ServerSocket(puerto);)
        {
            while (true)
            {
                Socket clienteSocket = serverSocket.accept();

                Manejo_Cliente hilo = new Manejo_Cliente(clienteSocket);
                hilo.start();
                
            }
        }

    catch (Exception e)
    {
        e.printStackTrace();
    }
    }
    
}
//Fin de la clase Socket
