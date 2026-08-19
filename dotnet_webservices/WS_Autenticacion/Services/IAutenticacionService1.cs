using System.ServiceModel;
using System.Runtime.Serialization;

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

    [ServiceContract(Namespace = "http://centraltelefonica.cr/ws/autenticacion")]
    public interface IAutenticacionService
    {
        [OperationContract]
        ResultadoOperacion AutenticarUsuario(string usuarioEncriptado, string contrasenaEncriptada, int tipo);

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
    }
}
