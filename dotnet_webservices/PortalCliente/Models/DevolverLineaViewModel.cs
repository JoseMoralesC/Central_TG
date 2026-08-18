namespace PortalCliente.Models;

public class DevolverLineaViewModel
{
    public string? Identificacion { get; set; }

    public bool ResultadoConsulta { get; set; }

    public string? MensajeConsulta { get; set; }

    public List<LineaDevolucion> LineasPrepago { get; set; } = new();

    public List<LineaDevolucion> LineasPostpago { get; set; } = new();

    public bool Procesado { get; set; }

    public bool Exitoso { get; set; }

    public string? MensajeResultado { get; set; }
}

public class LineaDevolucion
{
    public string NumeroTelefono { get; set; } = string.Empty;

    public string TipoServicio { get; set; } = string.Empty;

    public decimal Saldo { get; set; }

    public string? SaldoDetalle { get; set; }

    public decimal FacturaPendiente { get; set; }
}