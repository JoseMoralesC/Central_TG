namespace PortalCliente.Models;

public class ListarLineasDisponiblesResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public List<LineaDisponible> Lineas { get; set; } = new();
}
