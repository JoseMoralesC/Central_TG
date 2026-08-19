using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IAutenticacionPortalService
{
    Task<MetodoPagoCliente> ObtenerMetodoPagoClienteAsync(string identificacion);
}
