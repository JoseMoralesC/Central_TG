using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class RespuestaLlamada
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "RESPUESTA_LLAMADA";

        [JsonPropertyName("telefono_origen")]
        public string TelefonoOrigen { get; set; } = string.Empty;

        [JsonPropertyName("resultado")]
        public ResultadoLlamada Resultado { get; set; } = new();

        [JsonPropertyName("datos_llamada")]
        public DatosRespuestaLlamada DatosLlamada { get; set; } = new();
    }

    public class ResultadoLlamada
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }

    public class DatosRespuestaLlamada
    {
        [JsonPropertyName("id_llamada")]
        public string IdLlamada { get; set; } = string.Empty;

        [JsonPropertyName("tiempo_maximo_segundos")]
        public int TiempoMaximoSegundos { get; set; }

        [JsonPropertyName("hora_inicio")]
        public string HoraInicio { get; set; } = string.Empty;

        [JsonPropertyName("hora_fin_maxima")]
        public string HoraFinMaxima { get; set; } = string.Empty;
    }
}