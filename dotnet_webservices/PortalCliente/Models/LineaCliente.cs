namespace PortalCliente.Models;

public class LineaCliente
{
    public string NumeroTelefono { get; set; } = string.Empty;

    public string TipoServicio { get; set; } = string.Empty;

    public decimal Saldo { get; set; }

    public decimal FacturaPendiente { get; set; }

    public DateTime? FacturaFechaMaximaPago { get; set; }

    public bool Activo { get; set; }

    public string IdentificadorTelefono { get; set; } = string.Empty;

    public string IdentificadorTarjeta { get; set; } = string.Empty;

    public string IdentificacionDuenoCifrada { get; set; } = string.Empty;
}