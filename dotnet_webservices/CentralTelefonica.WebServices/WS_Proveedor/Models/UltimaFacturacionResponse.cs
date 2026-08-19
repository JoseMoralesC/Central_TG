using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class UltimaFacturacionResponse
    {
        [DataMember(Order = 1)]
        public bool Resultado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }

        [DataMember(Order = 3)]
        public bool HayFacturacion { get; set; }

        [DataMember(Order = 4)]
        public string FechaCalculo { get; set; }

        [DataMember(Order = 5)]
        public string FechaMaximaPago { get; set; }

        [DataMember(Order = 6)]
        public int TotalLineas { get; set; }

        [DataMember(Order = 7)]
        public int TotalLlamadas { get; set; }

        [DataMember(Order = 8)]
        public decimal TotalFacturar { get; set; }

        [DataMember(Order = 9)]
        public string FechaRegistro { get; set; }

        [DataMember(Order = 10)]
        public string NumeroTelefono { get; set; }
    }
}
