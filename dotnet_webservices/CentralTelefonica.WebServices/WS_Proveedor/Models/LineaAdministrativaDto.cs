using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class LineaAdministrativaDto
    {
        [DataMember(Order = 1)]
        public int ServicioId { get; set; }

        [DataMember(Order = 2)]
        public string NumeroTelefono { get; set; }

        [DataMember(Order = 3)]
        public string IdentificadorTelefono { get; set; }

        [DataMember(Order = 4)]
        public string IdentificadorTarjeta { get; set; }

        [DataMember(Order = 5)]
        public string TipoServicio { get; set; }

        [DataMember(Order = 6)]
        public string IdentificacionCliente { get; set; }

        [DataMember(Order = 7)]
        public string IdentificacionClienteVisible { get; set; }

        [DataMember(Order = 8)]
        public string NombreCliente { get; set; }

        [DataMember(Order = 9)]
        public string EstadoLinea { get; set; }

        [DataMember(Order = 10)]
        public bool Activo { get; set; }

        [DataMember(Order = 11)]
        public string ProveedorCodigo { get; set; }
    }
}
