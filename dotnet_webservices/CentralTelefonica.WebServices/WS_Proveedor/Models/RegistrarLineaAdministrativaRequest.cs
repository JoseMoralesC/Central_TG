using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class RegistrarLineaAdministrativaRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string NumeroTelefono { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string IdentificadorTelefono { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string IdentificadorTarjeta { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string TipoServicio { get; set; }
    }
}
