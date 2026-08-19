using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class SolicitudLineaDto
    {
        [DataMember(Order = 1)]
        public int SolicitudId { get; set; }

        [DataMember(Order = 2)]
        public int ServicioId { get; set; }

        [DataMember(Order = 3)]
        public string NumeroTelefono { get; set; }

        [DataMember(Order = 4)]
        public string TipoServicio { get; set; }

        [DataMember(Order = 5)]
        public string IdentificacionCliente { get; set; }

        [DataMember(Order = 6)]
        public string NombreCliente { get; set; }

        [DataMember(Order = 7)]
        public string Estado { get; set; }

        [DataMember(Order = 8)]
        public string FechaSolicitud { get; set; }
    }
}
