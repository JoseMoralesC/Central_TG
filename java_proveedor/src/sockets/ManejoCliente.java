package java_proveedor.src.sockets;

//Imports
import java.net.Socket;
import java.io.*;
import java.nio.charset.StandardCharsets;

public class ManejoCliente extends Thread
{
    //Atributos
    private Socket clienteSocket;

    public ManejoCliente(Socket clienteSocket)
    {
        this.clienteSocket = clienteSocket;
    }
    
}
