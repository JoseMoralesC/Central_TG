using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IProveedor2Service
{
    Task<ListarLineasDisponiblesResult> ListarLineasDisponiblesAsync();

    Task<CambioEstadoLineaResult> SolicitarLineaClienteAsync(
        int servicioId,
        string numeroTelefono,
        string tipoServicio,
        string identificacionCliente,
        string nombreCliente);

    Task<CambioEstadoLineaResult> ActivarDesactivarLineaAsync(
        ActivarDesactivarLineaPortalRequest solicitud);
}
