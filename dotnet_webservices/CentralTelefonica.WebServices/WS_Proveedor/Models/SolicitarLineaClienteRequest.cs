using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class SolicitarLineaClienteRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int ServicioId { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string NumeroTelefono { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string TipoServicio { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string IdentificacionCliente { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public string NombreCliente { get; set; }
    }
}
