using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WS_ProveedorCliente.Models
{
    [DataContract]
    public class ConsultarLineasClienteResponse
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public List<LineaClienteDto> Lineas { get; set; }
    }
}