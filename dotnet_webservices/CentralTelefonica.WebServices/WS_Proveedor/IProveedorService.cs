using System.ServiceModel;
using WS_Proveedor.Models;

namespace WS_Proveedor
{
    [ServiceContract]
    public interface IProveedorService
    {
        [OperationContract]
        RespuestaServicio ActivarDesactivarLinea(
            ActivarDesactivarLineaRequest solicitud
        );

        [OperationContract]
        FacturacionConsultaResponse ConsultarFacturacion(
            CalcularFacturacionRequest solicitud
        );

        [OperationContract]
        RespuestaServicio CalcularFacturacion(
            CalcularFacturacionRequest solicitud
        );

        [OperationContract]
        UltimaFacturacionResponse ObtenerUltimaFacturacion();

        [OperationContract]
        ListadoLineasResponse ListarLineasDisponibles();

        [OperationContract]
        RespuestaServicio SolicitarLineaCliente(SolicitarLineaClienteRequest solicitud);

        [OperationContract]
        ListadoSolicitudesLineaResponse ListarSolicitudesLineaPendientes();

        [OperationContract]
        RespuestaServicio MarcarSolicitudLineaAtendida(int solicitudId, string estado);

        [OperationContract]
        RespuestaServicio ActualizarCorreoCliente(ActualizarCorreoClienteRequest solicitud);

        [OperationContract]
        ListadoLineasResponse ListarLineasActivas();

        [OperationContract]
        RespuestaServicio RegistrarLinea(RegistrarLineaAdministrativaRequest solicitud);

        [OperationContract]
        RespuestaServicio EliminarLineaDisponible(int servicioId);
    }
}
