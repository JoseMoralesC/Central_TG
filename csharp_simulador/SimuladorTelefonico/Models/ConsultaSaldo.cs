using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class ConsultaSaldo
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "CONSULTA_SALDO";

        [JsonPropertyName("accion")]
        public string Accion { get; set; } = "SALDO";

        [JsonPropertyName("telefono_origen")]
        public string TelefonoOrigen { get; set; } = string.Empty;

        [JsonPropertyName("identificador_telefono")]
        public string IdentificadorTelefono { get; set; } = string.Empty;

        [JsonPropertyName("identificador_dispositivo")]
        public string IdentificadorDispositivo { get; set; } = string.Empty;

        [JsonPropertyName("identificador_tarjeta")]
        public string IdentificadorTarjeta { get; set; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public UbicacionTelefono Ubicacion { get; set; } = new();

        [JsonPropertyName("fecha_hora")]
        public string FechaHora { get; set; } = string.Empty;
    }
}
