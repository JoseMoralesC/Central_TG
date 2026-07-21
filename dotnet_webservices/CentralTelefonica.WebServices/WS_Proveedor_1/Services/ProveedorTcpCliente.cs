using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace WS_Proveedor_1.Services
{
    public class ProveedorTcpCliente
    {
        private readonly string host;
        private readonly int puerto;

        public ProveedorTcpCliente(string host = "127.0.0.1", int puerto = 5000)
        {
            this.host = host;
            this.puerto = puerto;
        }

        public string Enviar(string mensaje)
        {
            using (TcpClient cliente = new TcpClient())
            {
                // Espera máxima de 5 segundos
                cliente.ReceiveTimeout = 5000;
                cliente.SendTimeout = 5000;

                cliente.Connect(host, puerto);

                using (NetworkStream stream = cliente.GetStream())
                {
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje + "\n");

                    stream.Write(datos, 0, datos.Length);
                    stream.Flush();

                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadLine();
                    }
                }
            }
        }
    }
}