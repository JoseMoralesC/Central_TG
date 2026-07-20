using System.Text.RegularExpressions;
using CentralTelefonica.WebServices.WS_Autenticacion.Models;

public static class UsuarioValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex NombreRegex = new(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{14}$", RegexOptions.Compiled);

    public static string? Validar(Usuario usuario)
    {
        if (usuario is null)
        {
            return "El usuario no puede ser nulo.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Identificacion))
        {
            return "La identificación es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Nombre) || !NombreRegex.IsMatch(usuario.Nombre))
        {
            return "El nombre es inválido.";
        }

        if (string.IsNullOrWhiteSpace(usuario.PrimerApellido) || !NombreRegex.IsMatch(usuario.PrimerApellido))
        {
            return "El primer apellido es inválido.";
        }

        if (!string.IsNullOrWhiteSpace(usuario.SegundoApellido) && !NombreRegex.IsMatch(usuario.SegundoApellido))
        {
            return "El segundo apellido es inválido.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Correo) || !EmailRegex.IsMatch(usuario.Correo))
        {
            return "El correo es inválido.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Usuario))
        {
            return "El nombre de usuario es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Contrasena) || !PasswordRegex.IsMatch(usuario.Contrasena))
        {
            return "La contraseña debe tener exactamente 14 caracteres, incluir mayúscula, minúscula, número y carácter especial.";
        }

        if (usuario.Tipo != 1 && usuario.Tipo != 2)
        {
            return "El tipo de usuario debe ser 1 o 2.";
        }

        if (!string.IsNullOrWhiteSpace(usuario.Estado) && usuario.Estado.ToLowerInvariant() != "activo" && usuario.Estado.ToLowerInvariant() != "inactivo")
        {
            return "El estado debe ser activo o inactivo.";
        }

        return null;
    }
}