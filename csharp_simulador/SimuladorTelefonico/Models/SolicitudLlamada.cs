using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class SolicitudLlamada
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "SOLICITUD_LLAMADA";

        [JsonPropertyName("telefono_origen")]
        public string TelefonoOrigen { get; set; } = string.Empty;

        [JsonPropertyName("telefono_destino")]
        public string TelefonoDestino { get; set; } = string.Empty;

        [JsonPropertyName("identificador_dispositivo")]
        public string IdentificadorDispositivo { get; set; } = string.Empty;

        [JsonPropertyName("identificador_tarjeta")]
        public string IdentificadorTarjeta { get; set; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public UbicacionTelefono Ubicacion { get; set; } = new();

        [JsonPropertyName("tipo_llamada")]
        public string TipoLlamada { get; set; } = "NACIONAL";

        [JsonPropertyName("fecha_hora")]
        public string FechaHora { get; set; } = string.Empty;
    }

    public class UbicacionTelefono
    {
        [JsonPropertyName("pais")]
        public string Pais { get; set; } = "Costa Rica";

        [JsonPropertyName("provincia")]
        public string Provincia { get; set; } = "San Jose";

        [JsonPropertyName("latitud")]
        public double Latitud { get; set; } = 9.9281;

        [JsonPropertyName("longitud")]
        public double Longitud { get; set; } = -84.0907;
    }
}