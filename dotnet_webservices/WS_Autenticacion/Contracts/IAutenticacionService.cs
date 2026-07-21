using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using WS_Autenticacion.Models;

namespace WS_Autenticacion.Contracts
{
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
}