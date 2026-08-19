namespace PortalCliente.Models;

public class ConsultarLineasClienteResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public List<LineaCliente> Lineas { get; set; } = new();
}