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
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(clienteSocket.getInputStream(), StandardCharsets.UTF_8));
            PrintWriter writer = new PrintWriter(new OutputStreamWriter(clienteSocket.getOutputStream(), StandardCharsets.UTF_8), true))
        {

        }

        catch (Exception e)
        {
            e.printStackTrace();
        }

    }
     
}
