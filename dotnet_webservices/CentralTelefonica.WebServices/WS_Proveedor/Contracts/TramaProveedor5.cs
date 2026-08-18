using System.Runtime.Serialization;

namespace WS_Proveedor.Contracts
{
    [DataContract]
    public class TramaProveedor5
    {
        [DataMember(Name = "tipo_transaccion", Order = 1)]
        public string TipoTransaccion { get; set; }

        [DataMember(Name = "telefono", Order = 2)]
        public string Telefono { get; set; }

        [DataMember(Name = "identificador_dispositivo", Order = 3)]
        public string IdentificadorDispositivo { get; set; }

        [DataMember(Name = "identificador_tarjeta", Order = 4)]
        public string IdentificadorTarjeta { get; set; }

        [DataMember(Name = "tipo_servicio", Order = 5)]
        public string TipoServicio { get; set; }

        [DataMember(Name = "identificacion_dueno", Order = 6)]
        public string IdentificacionDueno { get; set; }

        [DataMember(Name = "accion", Order = 7)]
        public string Accion { get; set; }

        [DataMember(Name = "fecha_hora", Order = 8)]
        public string FechaHora { get; set; }

        [DataMember(Name = "nombre_cliente", Order = 9, EmitDefaultValue = false)]
        public string NombreCliente { get; set; }

        [DataMember(Name = "correo_cliente", Order = 10, EmitDefaultValue = false)]
        public string CorreoCliente { get; set; }
    }
}
