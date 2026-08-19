namespace PortalCliente.Models;

public class MetodoPagoCliente
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public string NumeroTarjeta { get; set; } = string.Empty;

    public string NombreTarjeta { get; set; } = string.Empty;

    public string FechaVencimiento { get; set; } = string.Empty;

    public string CodigoSeguridad { get; set; } = string.Empty;
}
