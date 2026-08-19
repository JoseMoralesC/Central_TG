using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_Proveedor_1.Models
{
    [DataContract]
    public class RespuestaProveedor
    {
        [DataMember]
        public string tipo_transaccion { get; set; }

        [DataMember]
        public ResultadoProveedor resultado { get; set; }
    }
}