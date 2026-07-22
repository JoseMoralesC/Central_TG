using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_Proveedor_1.Models
{
    [DataContract]
    public class ConsultarSaldoRequest
    {
        [DataMember]
        public string NumeroTelefono { get; set; }

        [DataMember]
        public string Origen { get; set; }

        [DataMember]
        public string TipoTransaccion { get; set; }

        [DataMember]
        public string IdentificadorTelefono { get; set; }

        [DataMember]
        public string IdentificadorTarjeta { get; set; }

        [DataMember]
        public string Pais { get; set; }

        [DataMember]
        public string Provincia { get; set; }

        [DataMember]
        public string Latitud { get; set; }

        [DataMember]
        public string Longitud { get; set; }
    }
}
