using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class ListadoLineasResponse
    {
        [DataMember(Order = 1)]
        public bool Resultado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }

        [DataMember(Order = 3)]
        public List<LineaAdministrativaDto> Lineas { get; set; }
    }
}
