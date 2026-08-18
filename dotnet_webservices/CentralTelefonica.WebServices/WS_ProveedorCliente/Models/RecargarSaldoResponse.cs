using System.Runtime.Serialization;

namespace WS_ProveedorCliente.Models
{
    [DataContract]
    public class RecargarSaldoResponse
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public decimal NuevoSaldo { get; set; }
    }
}