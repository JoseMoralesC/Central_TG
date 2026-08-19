using System.Runtime.Serialization;

namespace WS_Proveedor.Contracts
{
    [DataContract]
    public class TramaProveedor6
    {
        [DataMember(Name = "tipo_transaccion")]
        public string TipoTransaccion { get; set; } = "PROVEEDOR6";

        [DataMember(Name = "accion")]
        public string Accion { get; set; } = "CALCULAR_FACTURACION";

        [DataMember(Name = "fecha_calculo")]
        public string FechaCalculo { get; set; }

        [DataMember(Name = "fecha_maxima_pago")]
        public string FechaMaximaPago { get; set; }

        [DataMember(Name = "telefono_origen")]
        public string NumeroTelefono { get; set; }
    }
}
