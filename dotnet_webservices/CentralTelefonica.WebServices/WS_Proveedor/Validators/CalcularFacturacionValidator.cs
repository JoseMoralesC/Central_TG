using System;
using System.Text.RegularExpressions;
using WS_Proveedor.Models;

namespace WS_Proveedor.Validators
{
 public static class CalcularFacturacionValidator
 {
    private static readonly Regex FechaRegex = new Regex(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);

    public static bool EsValida(CalcularFacturacionRequest solicitud)
    {
    if (solicitud == null) return false;

    if (string.IsNullOrWhiteSpace(solicitud.FechaCalculo)) return false;
    if (string.IsNullOrWhiteSpace(solicitud.FechaMaximaPago)) return false;

    if (!FechaRegex.IsMatch(solicitud.FechaCalculo)) return false;
    if (!FechaRegex.IsMatch(solicitud.FechaMaximaPago)) return false;

    if (!DateTime.TryParse(solicitud.FechaCalculo, out _)) return false;
    if (!DateTime.TryParse(solicitud.FechaMaximaPago, out _)) return false;

    return true;
    }
}
}