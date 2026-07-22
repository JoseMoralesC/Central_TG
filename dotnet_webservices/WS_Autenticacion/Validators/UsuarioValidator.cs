using System.Text.RegularExpressions;
using WS_Autenticacion.Models;

namespace CentralTelefonica.WS_Autenticacion.Validators
{
    public static class UsuarioValidator
    {
        private static readonly Regex NombreRegex =
            new Regex(@"^(?!\s*$)[A-Za-zÁÉÍÓÚÑÜáéíóúñü\s]+$", RegexOptions.Compiled);

        private static readonly Regex ContrasenaRegex =
            new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{14}$", RegexOptions.Compiled);

        private static readonly Regex CorreoRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static bool EsNombreValido(string valor) =>
            !string.IsNullOrWhiteSpace(valor) && NombreRegex.IsMatch(valor.Trim());

        public static bool EsCorreoValido(string valor) =>
            !string.IsNullOrWhiteSpace(valor) && CorreoRegex.IsMatch(valor.Trim());

        public static bool EsContrasenaValida(string valorPlano) =>
            !string.IsNullOrEmpty(valorPlano) && ContrasenaRegex.IsMatch(valorPlano);

        public static bool EsTipoValido(int tipo) => tipo == 1 || tipo == 2;

        public static bool EsEstadoValido(string estado) =>
            estado == "activo" || estado == "inactivo";

        public static bool EsIdentificacionValida(string identificacion) =>
            !string.IsNullOrWhiteSpace(identificacion);
    }
}