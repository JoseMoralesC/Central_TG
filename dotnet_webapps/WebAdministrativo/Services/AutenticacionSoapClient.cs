using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WebAdministrativo.Services
{
    [ServiceContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public interface IAutenticacionService
    {
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
        ResultadoOperacion CambiarEstadoUsuario(string identificacion, string estado);

        [OperationContract]
        ResultadoListadoUsuarios ListarUsuariosPorTipo(int tipo);

        [OperationContract]
        ResultadoOperacion EliminarUsuario(string identificacion);
    }

    [DataContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public class ResultadoOperacion
    {
        [DataMember]
        public bool Resultado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
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

        public string UsuarioVisible { get; set; }

        public string ContrasenaVisible { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public int Tipo { get; set; }
    }

    public class AutenticacionSoapClient
    {
        private const int TipoAdministrador = 1;
        private const int TipoCliente = 2;

        public ResultadoAutenticacion LoginAdministrador(string usuario, string contrasena)
        {
            string usuarioEncriptado = CryptoHelper.Encrypt(usuario.Trim());
            string contrasenaEncriptada = CryptoHelper.Encrypt(contrasena);

            return Ejecutar(servicio =>
                servicio.AutenticarUsuarioDetalle(usuarioEncriptado, contrasenaEncriptada, TipoAdministrador));
        }

        public ResultadoListadoUsuarios ListarAdministradores()
        {
            return Ejecutar(servicio => servicio.ListarUsuariosPorTipo(TipoAdministrador));
        }

        public ResultadoListadoUsuarios ListarClientes()
        {
            return Ejecutar(servicio => servicio.ListarUsuariosPorTipo(TipoCliente));
        }

        public ResultadoOperacion CrearAdministrador(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correo,
            string usuario,
            string contrasena)
        {
            return Ejecutar(servicio =>
                servicio.CrearUsuario(
                    identificacion.Trim(),
                    nombre.Trim(),
                    primerApellido.Trim(),
                    string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim(),
                    correo.Trim(),
                    CryptoHelper.Encrypt(usuario.Trim()),
                    CryptoHelper.Encrypt(contrasena),
                    "activo",
                    TipoAdministrador));
        }

        public ResultadoOperacion ModificarAdministrador(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correo,
            string usuario,
            string contrasena)
        {
            return Ejecutar(servicio =>
                servicio.ModificarUsuario(
                    identificacion.Trim(),
                    nombre.Trim(),
                    primerApellido.Trim(),
                    string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim(),
                    correo.Trim(),
                    CryptoHelper.Encrypt(usuario.Trim()),
                    CryptoHelper.Encrypt(contrasena)));
        }

        public ResultadoOperacion ModificarAdministradorConUsuarioEncriptado(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correo,
            string usuarioEncriptado,
            string contrasena)
        {
            string contrasenaEncriptada = string.IsNullOrWhiteSpace(contrasena)
                ? null
                : CryptoHelper.Encrypt(contrasena);

            return Ejecutar(servicio =>
                servicio.ModificarUsuario(
                    identificacion.Trim(),
                    nombre.Trim(),
                    primerApellido.Trim(),
                    string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim(),
                    correo.Trim(),
                    usuarioEncriptado,
                    contrasenaEncriptada));
        }

        public ResultadoOperacion CambiarEstadoAdministrador(string identificacion, string estado)
        {
            return Ejecutar(servicio => servicio.CambiarEstadoUsuario(identificacion.Trim(), estado));
        }

        public ResultadoOperacion EliminarAdministrador(string identificacion)
        {
            return Ejecutar(servicio => servicio.EliminarUsuario(identificacion.Trim()));
        }

        private static T Ejecutar<T>(Func<IAutenticacionService, T> accion)
        {
            var url = ConfigurationManager.AppSettings["WsAutenticacionUrl"];

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ConfigurationErrorsException("Falta WsAutenticacionUrl en Web.config");
            }

            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 262144,
                SendTimeout = TimeSpan.FromSeconds(15),
                ReceiveTimeout = TimeSpan.FromSeconds(15),
                OpenTimeout = TimeSpan.FromSeconds(10),
                CloseTimeout = TimeSpan.FromSeconds(10)
            };

            var endpoint = new EndpointAddress(url);
            var factory = new ChannelFactory<IAutenticacionService>(binding, endpoint);
            IClientChannel channel = null;

            try
            {
                var proxy = factory.CreateChannel();
                channel = (IClientChannel)proxy;
                var resultado = accion(proxy);
                channel.Close();
                factory.Close();
                return resultado;
            }
            catch
            {
                channel?.Abort();
                factory.Abort();
                throw;
            }
        }
    }
}
