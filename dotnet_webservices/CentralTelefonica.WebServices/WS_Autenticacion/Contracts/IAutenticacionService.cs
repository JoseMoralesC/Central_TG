using System.ServiceModel;
using CentralTelefonica.WebServices.WS_Autenticacion.Models;

[ServiceContract]
public interface IAutenticacionService
{
    [OperationContract]
    AutenticacionResponse AutenticarUsuario(string usuario, string contrasena, int tipo);

    [OperationContract]
    AutenticacionResponse CrearUsuario(Usuario usuario);

    [OperationContract]
    AutenticacionResponse ModificarUsuario(Usuario usuario);

    [OperationContract]
    AutenticacionResponse CambiarEstadoUsuario(string usuario, string estado);
}