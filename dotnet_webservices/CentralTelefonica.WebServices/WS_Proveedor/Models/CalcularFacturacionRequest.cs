using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class CalcularFacturacionRequest
    {
        [DataMember]
        public string FechaCalculo { get; set; }

        [DataMember]
        public string FechaMaximaPago { get; set; }
    }
}