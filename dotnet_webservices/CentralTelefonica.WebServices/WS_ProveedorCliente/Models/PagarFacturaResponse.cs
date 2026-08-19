using System;
using System.Runtime.Serialization;

namespace WS_ProveedorCliente.Models
{
    [DataContract]
    public class PagarFacturaResponse
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string CorreoCliente { get; set; }

        [DataMember]
        public int FacturacionId { get; set; }

        [DataMember]
        public decimal MontoCancelado { get; set; }

        [DataMember]
        public DateTime FechaPago { get; set; }
    }
}