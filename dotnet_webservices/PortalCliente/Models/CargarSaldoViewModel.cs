namespace PortalCliente.Models;

public class CargarSaldoViewModel
{
    public bool ModoLista { get; set; }

    public string? Identificacion { get; set; }

    public bool ResultadoConsulta { get; set; }

    public string? MensajeConsulta { get; set; }

    public List<LineaCliente> Lineas { get; set; } = new();

    public string? Numero { get; set; }

    public string? NumeroTarjeta { get; set; }

    public string? NombreTarjeta { get; set; }

    public string? FechaVencimiento { get; set; }

    public string? CodigoSeguridad { get; set; }

    public int? Monto { get; set; }

    public bool Procesado { get; set; }

    public bool Exitoso { get; set; }

    public string? MensajeResultado { get; set; }

    public decimal NuevoSaldo { get; set; }
}