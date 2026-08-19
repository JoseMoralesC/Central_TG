namespace PortalCliente.Models;

public class SolicitarLineaViewModel
{
    public string? Identificacion { get; set; }

    public string? NombreCliente { get; set; }

    public bool ResultadoConsulta { get; set; }

    public string? MensajeConsulta { get; set; }

    public bool Procesado { get; set; }

    public bool Exitoso { get; set; }

    public string? MensajeResultado { get; set; }

    public List<LineaDisponible> Lineas { get; set; } = new();
}
