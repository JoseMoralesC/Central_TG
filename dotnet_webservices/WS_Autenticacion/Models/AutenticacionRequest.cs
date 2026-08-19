using System.Runtime.Serialization;

namespace WS_Autenticacion.Models
{
    [DataContract]
    public class AutenticacionRequest
    {
        [DataMember(Order = 1)]
        public string Usuario { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string Contrasena { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public int Tipo { get; set; }
    }
}
