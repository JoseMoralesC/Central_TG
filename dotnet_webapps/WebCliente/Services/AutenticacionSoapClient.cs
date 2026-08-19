using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WebCliente.Services
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
        ResultadoOperacion RegistrarMetodoPagoCliente(
            string identificacion,
            string numeroTarjetaEncriptado,
            string nombreTarjetaEncriptado,
            string fechaVencimientoEncriptada,
            string codigoSeguridadEncriptado);
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

    public class AutenticacionSoapClient
    {
        private const int TipoCliente = 2;

        public ResultadoAutenticacion LoginCliente(string usuario, string contrasena)
        {
            string usuarioEncriptado = CryptoHelper.Encrypt(usuario.Trim());
            string contrasenaEncriptada = CryptoHelper.Encrypt(contrasena);

            return Ejecutar(servicio =>
                servicio.AutenticarUsuarioDetalle(usuarioEncriptado, contrasenaEncriptada, TipoCliente));
        }

        public ResultadoOperacion RegistrarCliente(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correo,
            string usuario,
            string contrasena)
        {
            string usuarioEncriptado = CryptoHelper.Encrypt(usuario.Trim());
            string contrasenaEncriptada = CryptoHelper.Encrypt(contrasena);

            return Ejecutar(servicio =>
                servicio.CrearUsuario(
                    identificacion.Trim(),
                    nombre.Trim(),
                    primerApellido.Trim(),
                    string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim(),
                    correo.Trim(),
                    usuarioEncriptado,
                    contrasenaEncriptada,
                    "activo",
                    TipoCliente));
        }

        public ResultadoOperacion RegistrarMetodoPagoCliente(
            string identificacion,
            string numeroTarjeta,
            string nombreTarjeta,
            string fechaVencimiento,
            string codigoSeguridad)
        {
            return Ejecutar(servicio =>
                servicio.RegistrarMetodoPagoCliente(
                    identificacion.Trim(),
                    CryptoHelper.Encrypt(numeroTarjeta.Trim()),
                    CryptoHelper.Encrypt(nombreTarjeta.Trim()),
                    CryptoHelper.Encrypt(fechaVencimiento.Trim()),
                    CryptoHelper.Encrypt(codigoSeguridad.Trim())));
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
                MaxReceivedMessageSize = 65536,
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
