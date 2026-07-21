using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_Proveedor_1.Models
{
    public class Proovedor_telefono
    {
        internal string Enviar(string json)
        {
            throw new NotImplementedException();
        }

        [DataContract]
        public class AgregarTelefono

        {
            [DataMember(Order = 1, IsRequired = true)]
            public string NumeroTelefono { get; set; }

            [DataMember(Order = 2, IsRequired = true)]
            public string IdentificadorTelefono { get; set; }

            [DataMember(Order = 3, IsRequired = true)]
            public string IdentificadorTarjeta { get; set; }

            [DataMember(Order = 4, IsRequired = true)]
            public string Tipo { get; set; }

            [DataMember(Order = 6, IsRequired = true)]
            public string Estado { get; set; }
        }
    }
}