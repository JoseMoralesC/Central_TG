using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;

namespace WS_Proveedor.Models
{
    [DataContract]
    public class ActivarDesactivarLineaRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string NumeroTelefono { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string IdentificadorTelefono { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string IdentificadorTarjeta { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string Tipo { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public string IdentificacionCliente { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public string Estado { get; set; }
    }
}