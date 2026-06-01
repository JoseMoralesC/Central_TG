using System.Text.Json;
using System.Threading.Tasks;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Socket;

namespace SimuladorTelefonico.Services
{
    public class TramaService
    {
        private readonly JsonSerializerOptions _opcionesJson;
        private readonly BitacoraService _bitacoraService;

        public TramaService()
        {
            _opcionesJson = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _bitacoraService = new BitacoraService();
        }

        public string ConvertirAJson<T>(T trama)
        {
            return JsonSerializer.Serialize(trama, _opcionesJson);
        }

        public async Task<string> EnviarTramaAsync<T>(T trama)
        {
            string json = ConvertirAJson(trama);

            _bitacoraService.RegistrarEvento("TRAMA ENVIADA", json);

            TcpSocketClient cliente = new TcpSocketClient(
                AppConfig.HostIdentificador,
                AppConfig.PuertoIdentificador
            );

            string respuesta = await cliente.EnviarTramaAsync(json);

            _bitacoraService.RegistrarEvento("RESPUESTA RECIBIDA", respuesta);

            return respuesta;
        }
    }
}