namespace PortalCliente.Models;

public class ActivarDesactivarLineaPortalRequest
{
    public string NumeroTelefono { get; set; } = string.Empty;

    public string IdentificadorTelefono { get; set; } = string.Empty;

    public string IdentificadorTarjeta { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public string IdentificacionCliente { get; set; } = string.Empty;

    public string Estado { get; set; } = "inactivo";
}

public class CambioEstadoLineaResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}