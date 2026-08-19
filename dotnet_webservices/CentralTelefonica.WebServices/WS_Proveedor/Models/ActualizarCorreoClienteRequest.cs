using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class ActualizarCorreoClienteRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string IdentificacionCliente { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string CorreoCliente { get; set; }
    }
}
