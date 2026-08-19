using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IProveedorPortalService
{
    Task<ConsultarSaldoPortalResult> ConsultarSaldoAsync(string numeroTelefono);
}