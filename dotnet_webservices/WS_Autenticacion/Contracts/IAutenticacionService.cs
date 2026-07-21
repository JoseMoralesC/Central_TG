using System.ServiceModel;
using WS_Autenticacion.Models;

namespace WS_Autenticacion.Contracts
{
    [ServiceContract]
    public interface IAutenticacionService
    {
        [OperationContract]
        AutenticacionResponse AutenticarUsuario(AutenticacionRequest request);

        [OperationContract]
        AutenticacionResponse CrearUsuario(Usuario usuario);

        [OperationContract]
        AutenticacionResponse ModificarUsuario(Usuario usuario);

        [OperationContract]
        AutenticacionResponse CambiarEstadoUsuario(string usuario, string estado);
    }
}