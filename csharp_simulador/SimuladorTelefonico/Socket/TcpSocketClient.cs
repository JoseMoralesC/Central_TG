using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimuladorTelefonico.Config;

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
            if (string.IsNullOrWhiteSpace(tramaJson))
            {
                return "ERROR: La trama JSON esta vacia.";
            }

            try
            {
                using TcpClient client = new TcpClient();
                using CancellationTokenSource ctsConexion =
                    new(AppConfig.TimeoutConexionMs);

                await client.ConnectAsync(_host, _port, ctsConexion.Token);

                using NetworkStream stream = client.GetStream();
                stream.ReadTimeout = AppConfig.TimeoutLecturaMs;
                stream.WriteTimeout = AppConfig.TimeoutConexionMs;

                Encoding encoding = ObtenerEncoding();
                string mensaje = tramaJson.Trim() + "\n";
                byte[] data = encoding.GetBytes(mensaje);

                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();

                byte[] buffer = new byte[AppConfig.BufferRespuestaBytes];
                using CancellationTokenSource ctsLectura =
                    new(AppConfig.TimeoutLecturaMs);

                int bytesLeidos = await stream.ReadAsync(buffer, ctsLectura.Token);

                if (bytesLeidos <= 0)
                {
                    return "ERROR: No se recibio respuesta del Identificador Python.";
                }

                string respuesta = encoding.GetString(buffer, 0, bytesLeidos);

                return respuesta.Trim();
            }
            catch (SocketException)
            {
                return
                    $"ERROR: No fue posible conectar con el Identificador Python en {_host}:{_port}.";
            }
            catch (OperationCanceledException)
            {
                return
                    $"ERROR: Tiempo de espera agotado al comunicarse con el Identificador Python en {_host}:{_port}.";
            }
            catch (IOException)
            {
                return "ERROR: Se interrumpio la comunicacion con el Identificador Python.";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        private static Encoding ObtenerEncoding()
        {
            try
            {
                return Encoding.GetEncoding(AppConfig.SocketEncoding);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }
    }
}
