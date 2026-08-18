namespace PortalCliente.Models;

public class PagarFacturaResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public string CorreoCliente { get; set; } = string.Empty;

    public int FacturacionId { get; set; }

    public decimal MontoCancelado { get; set; }

    public DateTime FechaPago { get; set; }
}