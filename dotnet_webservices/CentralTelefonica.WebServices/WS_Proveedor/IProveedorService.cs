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
        RespuestaServicio CalcularFacturacion(
            CalcularFacturacionRequest solicitud
        );

        [OperationContract]
        UltimaFacturacionResponse ObtenerUltimaFacturacion();
    }
}
