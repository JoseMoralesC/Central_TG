namespace PortalCliente.Models;

public class ConsultarSaldoPortalResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public decimal Saldo { get; set; }
}