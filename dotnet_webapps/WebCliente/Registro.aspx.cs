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
            new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{7,}$", RegexOptions.Compiled);

        private static readonly Regex TarjetaRegex =
            new Regex(@"^\d{12}$", RegexOptions.Compiled);

        private static readonly Regex VencimientoRegex =
            new Regex(@"^(0[1-9]|1[0-2])/\d{2}$", RegexOptions.Compiled);

        private static readonly Regex CodigoSeguridadRegex =
            new Regex(@"^\d{3}$", RegexOptions.Compiled);

        protected void RegistrarButton_Click(object sender, EventArgs e)
        {
            string error = ValidarFormulario();
            if (!string.IsNullOrWhiteSpace(error))
            {
                MensajeLabel.Text = error;
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

                if (respuesta == null || !respuesta.Resultado)
                {
                    MensajeLabel.Text = respuesta?.Mensaje ?? "Error al realizar el proceso";
                    return;
                }

                var metodoPago = _autenticacionClient.RegistrarMetodoPagoCliente(
                    IdentificacionText.Text,
                    NormalizarTarjeta(NumeroTarjetaText.Text),
                    NombreTarjetaText.Text,
                    FechaVencimientoText.Text,
                    CodigoSeguridadText.Text);

                MensajeLabel.Text = metodoPago != null && metodoPago.Resultado
                    ? "Registro exitoso"
                    : metodoPago?.Mensaje ?? "No se pudo registrar el metodo de pago.";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private string ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(IdentificacionText.Text) ||
                string.IsNullOrWhiteSpace(NombreText.Text) ||
                string.IsNullOrWhiteSpace(PrimerApellidoText.Text) ||
                string.IsNullOrWhiteSpace(UsuarioText.Text))
            {
                return "Complete los datos personales obligatorios.";
            }

            if (string.IsNullOrWhiteSpace(CorreoText.Text) ||
                !CorreoRegex.IsMatch(CorreoText.Text.Trim()))
            {
                return "Ingrese un correo electronico valido.";
            }

            if (!ContrasenaRegex.IsMatch(ContrasenaText.Text))
            {
                return "La contrasena debe tener minimo 7 caracteres, con mayuscula, minuscula, numero y especial.";
            }

            if (!TarjetaRegex.IsMatch(NormalizarTarjeta(NumeroTarjetaText.Text)))
            {
                return "El numero de tarjeta debe tener doce digitos.";
            }

            if (string.IsNullOrWhiteSpace(NombreTarjetaText.Text))
            {
                return "Debe ingresar el nombre del dueno de la tarjeta.";
            }

            if (!VencimientoRegex.IsMatch(FechaVencimientoText.Text.Trim()) ||
                !VencimientoVigente(FechaVencimientoText.Text.Trim()))
            {
                return "La fecha de vencimiento debe tener formato MM/AA y estar vigente.";
            }

            if (!CodigoSeguridadRegex.IsMatch(CodigoSeguridadText.Text.Trim()))
            {
                return "El codigo de seguridad debe tener tres digitos.";
            }

            return string.Empty;
        }

        private static string NormalizarTarjeta(string valor)
        {
            return (valor ?? string.Empty)
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty);
        }

        private static bool VencimientoVigente(string fecha)
        {
            string[] partes = fecha.Split('/');
            int mes = int.Parse(partes[0]);
            int anio = 2000 + int.Parse(partes[1]);
            DateTime ahora = DateTime.Now;

            return anio > ahora.Year ||
                (anio == ahora.Year && mes >= ahora.Month);
        }
    }
}
