using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorTelefonico.Socket
{
    public class TcpSocketClient
    {
        private readonly string _host;
        private readonly int _port;

        public TcpSocketClient(string host = "127.0.0.1", int port = 5000)
        {
            _host = host;
            _port = port;
        }

        public async Task<string> EnviarTramaAsync(string tramaJson)
        {
            try
            {
                using TcpClient client = new TcpClient();

                await client.ConnectAsync(_host, _port);

                using NetworkStream stream = client.GetStream();

                string mensaje = tramaJson + "\n";
                byte[] data = Encoding.UTF8.GetBytes(mensaje);

                await stream.WriteAsync(data);
                await stream.FlushAsync();

                byte[] buffer = new byte[8192];
                StringBuilder respuestaCompleta = new StringBuilder();

                int bytesLeidos = await stream.ReadAsync(buffer);

                if (bytesLeidos <= 0)
                {
                    return "ERROR: No se recibió respuesta del Identificador Python.";
                }

                respuestaCompleta.Append(
                    Encoding.UTF8.GetString(buffer, 0, bytesLeidos)
                );

                return respuestaCompleta.ToString().Trim();
            }
            catch (SocketException)
            {
                return $"ERROR: No fue posible conectar con el Identificador Python en {_host}:{_port}.";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
    }
}