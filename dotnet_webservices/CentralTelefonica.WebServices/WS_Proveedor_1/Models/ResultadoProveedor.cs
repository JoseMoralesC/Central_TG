using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_Proveedor_1.Models
{
    [DataContract]
    public class ResultadoProveedor
    {
        [DataMember]
        public string codigo { get; set; }

        [DataMember]
        public string mensaje { get; set; }

    }
}