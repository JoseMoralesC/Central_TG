using System;
using System.Text.RegularExpressions;
using WebCliente.Services;

namespace WebCliente
{
    public partial class Registro : System.Web.UI.Page
    {
        private readonly AutenticacionSoapClient _autenticacionClient =
            new AutenticacionSoapClient();

        private static readonly Regex CorreoRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex ContrasenaRegex =
            new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{14}$", RegexOptions.Compiled);

        protected void RegistrarButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdentificacionText.Text) ||
                string.IsNullOrWhiteSpace(NombreText.Text) ||
                string.IsNullOrWhiteSpace(PrimerApellidoText.Text) ||
                string.IsNullOrWhiteSpace(CorreoText.Text) ||
                string.IsNullOrWhiteSpace(UsuarioText.Text) ||
                string.IsNullOrWhiteSpace(ContrasenaText.Text) ||
                !CorreoRegex.IsMatch(CorreoText.Text.Trim()) ||
                !ContrasenaRegex.IsMatch(ContrasenaText.Text))
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                var respuesta = _autenticacionClient.RegistrarCliente(
                    IdentificacionText.Text,
                    NombreText.Text,
                    PrimerApellidoText.Text,
                    SegundoApellidoText.Text,
                    CorreoText.Text,
                    UsuarioText.Text,
                    ContrasenaText.Text);

                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Registro exitoso"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }
    }
}
