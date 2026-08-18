using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IProveedor2Service
{
    Task<CambioEstadoLineaResult> ActivarDesactivarLineaAsync(
        ActivarDesactivarLineaPortalRequest solicitud);
}