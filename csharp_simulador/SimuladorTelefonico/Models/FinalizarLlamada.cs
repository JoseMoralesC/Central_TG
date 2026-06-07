using System.Text.Json.Serialization;

namespace SimuladorTelefonico.Models
{
    public class FinalizarLlamada
    {
        [JsonPropertyName("tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "FINALIZAR_LLAMADA";

        [JsonPropertyName("datos_llamada")]
        public DatosFinalizarLlamada DatosLlamada { get; set; } = new();

        [JsonPropertyName("datos_cobro")]
        public DatosCobroLlamada DatosCobro { get; set; } = new();
    }

    public class DatosFinalizarLlamada
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

        [JsonPropertyName("fecha_fin")]
        public string FechaFin { get; set; } = string.Empty;

        [JsonPropertyName("duracion_segundos")]
        public int DuracionSegundos { get; set; }

        [JsonPropertyName("duracion_minutos")]
        public int DuracionMinutos { get; set; }

        [JsonPropertyName("motivo_finalizacion")]
        public string MotivoFinalizacion { get; set; } = "FINALIZACION_MANUAL";
    }

    public class DatosCobroLlamada
    {
        [JsonPropertyName("tipo_servicio")]
        public string TipoServicio { get; set; } = "PREPAGO";

        [JsonPropertyName("tipo_llamada")]
        public string TipoLlamada { get; set; } = "NACIONAL";

        [JsonPropertyName("costo_por_minuto")]
        public decimal CostoPorMinuto { get; set; } = 10.00m;

        [JsonPropertyName("monto_total")]
        public decimal MontoTotal { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "CRC";
    }
}
