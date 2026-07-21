using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_Proveedor_1.Models
{
    [DataContract]
    public class Registrar_linea
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

    }
}