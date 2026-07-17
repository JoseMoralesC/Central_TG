using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class RespuestaServicio
    {
        [DataMember(Order = 1)]
        public bool Resultado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }
    }
}