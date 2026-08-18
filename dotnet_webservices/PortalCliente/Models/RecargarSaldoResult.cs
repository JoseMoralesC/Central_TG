namespace PortalCliente.Models;

public class RecargarSaldoResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public decimal NuevoSaldo { get; set; }
}