namespace PortalCliente.Models;

public class PagarFacturaViewModel
{
    public bool ModoLista { get; set; }

    public string? Identificacion { get; set; }

    public bool ResultadoConsulta { get; set; }

    public string? MensajeConsulta { get; set; }

    public List<LineaCliente> Lineas { get; set; } = new();

    public string? Numero { get; set; }

    public decimal MontoFactura { get; set; }

    public string? NumeroTarjeta { get; set; }

    public string? NombreTarjeta { get; set; }

    public string? FechaVencimiento { get; set; }

    public string? CodigoSeguridad { get; set; }

    public bool Procesado { get; set; }

    public bool Exitoso { get; set; }

    public string? MensajeResultado { get; set; }

    public string? DetalleCorreo { get; set; }
}