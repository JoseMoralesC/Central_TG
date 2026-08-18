using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IProveedorClienteService
{
    Task<ConsultarLineasClienteResult> ConsultarLineasClienteAsync(string identificacion);

    Task<RecargarSaldoResult> RecargarSaldoAsync(string numeroTelefono, decimal monto);

    Task<PagarFacturaResult> PagarFacturaAsync(
        string numeroTelefono,
        decimal monto,
        string identificacion);
}