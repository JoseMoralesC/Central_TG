using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class InicioLlamada
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "INICIO_LLAMADA";

        [JsonPropertyName("datos_llamada")]
        public DatosInicioLlamada DatosLlamada { get; set; } = new();

        [JsonPropertyName("control")]
        public ControlInicioLlamada Control { get; set; } = new();
    }

    public class DatosInicioLlamada
    {
        [JsonPropertyName("id_llamada")]
        public string IdLlamada { get; set; } = string.Empty;

        [JsonPropertyName("telefono_origen")]
        public string TelefonoOrigen { get; set; } = string.Empty;

        [JsonPropertyName("identificador_telefono")]
        public string IdentificadorTelefono { get; set; } = string.Empty;

        [JsonPropertyName("identificador_dispositivo")]
        public string IdentificadorDispositivo { get; set; } = string.Empty;

        [JsonPropertyName("identificador_tarjeta")]
        public string IdentificadorTarjeta { get; set; } = string.Empty;

        [JsonPropertyName("telefono_destino")]
        public string TelefonoDestino { get; set; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public UbicacionTelefono Ubicacion { get; set; } = new();

        [JsonPropertyName("fecha_inicio")]
        public string FechaInicio { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = "ACTIVA";
    }

    public class ControlInicioLlamada
    {
        [JsonPropertyName("tipo_servicio")]
        public string TipoServicio { get; set; } = "PREPAGO";

        [JsonPropertyName("tiempo_maximo_segundos")]
        public int TiempoMaximoSegundos { get; set; } = 1800;

        [JsonPropertyName("validar_saldo")]
        public bool ValidarSaldo { get; set; } = true;

        [JsonPropertyName("monitoreo_activo")]
        public bool MonitoreoActivo { get; set; } = true;
    }
}
