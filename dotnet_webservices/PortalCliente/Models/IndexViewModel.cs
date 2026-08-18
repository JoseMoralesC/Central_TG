namespace PortalCliente.Models;

public class IndexViewModel
{
    public bool RequiereIdentificacion { get; set; }

    public string? Identificacion { get; set; }

    public bool Resultado { get; set; }

    public string? Mensaje { get; set; }

    public List<LineaCliente> Lineas { get; set; } = new();

    public static IndexViewModel SinIdentificacion() =>
        new() { RequiereIdentificacion = true };
}