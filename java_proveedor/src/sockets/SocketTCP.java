//Package
package java_proveedor.src.sockets;

//Imports

import java.net.ServerSocket;
import java.net.Socket;

//Clase Socket
public class SocketTCP
{
    public static void main(String[] args)
    {
        try
    {
        //Servidor TCP escuchando en el puerto 6000
        ServerSocket serverSocket = new ServerSocket(6000);
        System.out.println("Servidor TCP escuchando...");

        while (true)
        {
            //Aceptar una conexión entrante
            Socket clientSocket = serverSocket.accept();
            System.out.println("Cliente conectado: " + clientSocket.getInetAddress());

            //Cerrar la conexión con el cliente
            clientSocket.close();
            serverSocket.close();

            break; // Salir del bucle después de manejar una conexión (opcional, dependiendo de si quieres manejar múltiples conexiones)
        }
    }

    catch (Exception e)
    {
        e.printStackTrace();
    }
    }
    
}
//Fin de la clase Socket
