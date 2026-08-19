using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IAutenticacionPortalService
{
    Task<MetodoPagoCliente> ObtenerMetodoPagoClienteAsync(string identificacion);

    Task<ClientePerfilResult> ObtenerPerfilClienteAsync(string identificacion);

    Task<OperacionPerfilResult> ActualizarPerfilClienteAsync(PerfilClienteViewModel perfil);
}
