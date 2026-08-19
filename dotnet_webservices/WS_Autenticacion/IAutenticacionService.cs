using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CentralTelefonica.WS_Autenticacion
{
    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class ResultadoOperacion
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        public static ResultadoOperacion Ok(string mensaje = "Exitoso")
            => new ResultadoOperacion { Resultado = true, Mensaje = mensaje };

        public static ResultadoOperacion Fallo(string mensaje)
            => new ResultadoOperacion { Resultado = false, Mensaje = mensaje };
    }

    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class UsuarioServicio
    {
        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string PrimerApellido { get; set; }

        [DataMember]
        public string SegundoApellido { get; set; }

        [DataMember]
        public string CorreoElectronico { get; set; }

        [DataMember]
        public string UsuarioEncriptado { get; set; }

        [DataMember]
        public string ContrasenaEncriptada { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public int Tipo { get; set; }
    }

    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class ResultadoAutenticacion
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public UsuarioServicio Usuario { get; set; }

        public static ResultadoAutenticacion Ok(UsuarioServicio usuario, string mensaje = "Exitoso")
            => new ResultadoAutenticacion { Resultado = true, Mensaje = mensaje, Usuario = usuario };

        public static ResultadoAutenticacion Fallo(string mensaje)
            => new ResultadoAutenticacion { Resultado = false, Mensaje = mensaje };
    }

    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class ResultadoListadoUsuarios
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public List<UsuarioServicio> Usuarios { get; set; }

        public static ResultadoListadoUsuarios Ok(List<UsuarioServicio> usuarios)
            => new ResultadoListadoUsuarios { Resultado = true, Mensaje = "Exitoso", Usuarios = usuarios };

        public static ResultadoListadoUsuarios Fallo(string mensaje)
            => new ResultadoListadoUsuarios { Resultado = false, Mensaje = mensaje, Usuarios = new List<UsuarioServicio>() };
    }

    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class MetodoPagoClienteServicio
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string NumeroTarjeta { get; set; }

        [DataMember]
        public string NombreTarjeta { get; set; }

        [DataMember]
        public string FechaVencimiento { get; set; }

        [DataMember]
        public string CodigoSeguridad { get; set; }

        public static MetodoPagoClienteServicio Fallo(string mensaje)
            => new MetodoPagoClienteServicio
            {
                Resultado = false,
                Mensaje = mensaje,
                NumeroTarjeta = string.Empty,
                NombreTarjeta = string.Empty,
                FechaVencimiento = string.Empty,
                CodigoSeguridad = string.Empty
            };
    }

    [ServiceContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public interface IAutenticacionService
    {
        [OperationContract]
        ResultadoOperacion AutenticarUsuario(string usuarioEncriptado, string contrasenaEncriptada, int tipo);

        [OperationContract]
        ResultadoAutenticacion AutenticarUsuarioDetalle(string usuarioEncriptado, string contrasenaEncriptada, int tipo);

        [OperationContract]
        ResultadoOperacion CrearUsuario(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correoElectronico,
            string usuarioEncriptado,
            string contrasenaEncriptada,
            string estado,
            int tipo);

        [OperationContract]
        ResultadoOperacion ModificarUsuario(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correoElectronico,
            string usuarioEncriptado,
            string contrasenaEncriptada);

        [OperationContract]
        ResultadoOperacion ModificarCliente(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correoElectronico,
            string usuarioEncriptado,
            string contrasenaEncriptada);

        [OperationContract]
        ResultadoOperacion CambiarEstadoUsuario(string identificacion, string estado);

        [OperationContract]
        ResultadoListadoUsuarios ListarUsuariosPorTipo(int tipo);

        [OperationContract]
        ResultadoOperacion EliminarUsuario(string identificacion);

        [OperationContract]
        ResultadoOperacion RegistrarMetodoPagoCliente(
            string identificacion,
            string numeroTarjetaEncriptado,
            string nombreTarjetaEncriptado,
            string fechaVencimientoEncriptada,
            string codigoSeguridadEncriptado);

        [OperationContract]
        MetodoPagoClienteServicio ObtenerMetodoPagoCliente(string identificacion);
    }
}
