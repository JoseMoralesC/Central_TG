using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class RespuestaSaldo
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "RESPUESTA_SALDO";

        [JsonPropertyName("telefono_origen")]
        public string TelefonoOrigen { get; set; } = string.Empty;

        [JsonPropertyName("resultado")]
        public ResultadoSaldo Resultado { get; set; } = new();

        [JsonPropertyName("saldo")]
        public DatosSaldo Saldo { get; set; } = new();
    }

    public class ResultadoSaldo
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }

    public class DatosSaldo
    {
        [JsonPropertyName("monto_disponible")]
        public decimal MontoDisponible { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "CRC";

        [JsonPropertyName("fecha_consulta")]
        public string FechaConsulta { get; set; } = string.Empty;
    }
}