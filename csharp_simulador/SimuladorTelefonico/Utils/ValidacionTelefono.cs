using System.Text.RegularExpressions;

namespace SimuladorTelefonico.Utils
{
    public static class ValidacionTelefono
    {
        public static bool EsNumeroValido(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return false;
            }

            return Regex.IsMatch(numero.Trim(), @"^[0-9]{8}$");
        }

        public static bool EsCodigoConsultaSaldo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            return valor.Trim() == "#9090*";
        }
    }
}