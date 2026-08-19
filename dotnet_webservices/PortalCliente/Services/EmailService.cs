using System.Net;
using System.Net.Mail;
using PortalCliente.Models;

namespace PortalCliente.Services;

public interface IEmailService
{
    Task<EmailEnvioResultado> EnviarFacturacionAsync(
        string correo,
        string numeroTelefono,
        PagarFacturaResult detalle);
}

public class EmailEnvioResultado
{
    public bool Enviado { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<EmailEnvioResultado> EnviarFacturacionAsync(
        string correo,
        string numeroTelefono,
        PagarFacturaResult detalle)
    {
        string host = _configuration["Smtp:Host"] ?? string.Empty;
        int puerto = int.TryParse(_configuration["Smtp:Port"], out int p) ? p : 587;
        string cuenta = _configuration["Smtp:User"] ?? string.Empty;
        string desde = _configuration["Smtp:From"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(desde))
        {
            desde = cuenta;
        }

        string clave = _configuration["Smtp:Password"] ?? string.Empty;
        bool cifrado = string.Equals(
            _configuration["Smtp:EnableSsl"],
            "false",
            StringComparison.OrdinalIgnoreCase) == false;

        if (string.IsNullOrWhiteSpace(correo)
            || string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(cuenta)
            || string.IsNullOrWhiteSpace(desde)
            || string.IsNullOrWhiteSpace(clave))
        {
            return new EmailEnvioResultado
            {
                Enviado = false,
                Mensaje = "No se configuro SMTP o el cliente no tiene correo registrado."
            };
        }

        string cuerpo = string.Join(
            Environment.NewLine,
            "Estimado(a) cliente:",
            "",
            "Central Telefonica TG confirma que la cancelacion de su factura postpago fue exitosa.",
            "Este es el recibo del pago realizado:",
            "",
            $"Numero de linea: {numeroTelefono}",
            $"Identificacion de factura: {detalle.FacturacionId}",
            $"Monto cancelado: {detalle.MontoCancelado:N2}",
            $"Fecha de pago: {detalle.FechaPago:dd/MM/yyyy HH:mm}",
            "",
            "Gracias por usar el portal del cliente.",
            "",
            "Central Telefonica TG");

        using var mensaje = new MailMessage(desde, correo)
        {
            Subject = "Recibo de pago de factura - Central Telefonica TG",
            Body = cuerpo
        };

        using var cliente = new SmtpClient(host, puerto);
        cliente.EnableSsl = cifrado;
        cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
        cliente.UseDefaultCredentials = false;

        if (!string.IsNullOrWhiteSpace(cuenta))
        {
            cliente.Credentials = new NetworkCredential(cuenta, clave);
        }

        try
        {
            await cliente.SendMailAsync(mensaje).ConfigureAwait(false);
            return new EmailEnvioResultado
            {
                Enviado = true,
                Mensaje = "Correo de recibo de pago enviado."
            };
        }
        catch (Exception ex)
        {
            return new EmailEnvioResultado
            {
                Enviado = false,
                Mensaje = "El pago se registro, pero no se pudo enviar el correo: "
                    + ex.Message
            };
        }
    }
}
