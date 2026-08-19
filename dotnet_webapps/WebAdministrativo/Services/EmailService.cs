using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace WebAdministrativo.Services
{
    public class EmailEnvioResultado
    {
        public bool Enviado { get; set; }

        public string Mensaje { get; set; }
    }

    public class FacturaGeneradaEmail
    {
        public string CorreoCliente { get; set; }

        public string NombreCliente { get; set; }

        public string NumeroTelefono { get; set; }

        public string FechaCalculo { get; set; }

        public string FechaMaximaPago { get; set; }

        public int TotalLlamadas { get; set; }

        public decimal TotalFacturar { get; set; }
    }

    public class EmailService
    {
        public EmailEnvioResultado EnviarFacturaGenerada(FacturaGeneradaEmail factura)
        {
            if (factura == null || string.IsNullOrWhiteSpace(factura.CorreoCliente))
            {
                return new EmailEnvioResultado
                {
                    Enviado = false,
                    Mensaje = "Factura generada. No se envio correo: el cliente no tiene correo registrado."
                };
            }

            var configuracion = SmtpConfiguracion.Leer();
            if (!configuracion.EstaCompleta)
            {
                return new EmailEnvioResultado
                {
                    Enviado = false,
                    Mensaje = "Factura generada. No se envio correo: falta configurar SMTP."
                };
            }

            string nombre = string.IsNullOrWhiteSpace(factura.NombreCliente)
                ? "cliente"
                : factura.NombreCliente.Trim();

            string cuerpo = string.Join(
                Environment.NewLine,
                "Estimado(a) " + nombre + ":",
                "",
                "Central Telefonica TG ha generado una nueva factura postpago con el siguiente detalle:",
                "",
                "Numero de linea: " + factura.NumeroTelefono,
                "Periodo facturado: " + factura.FechaCalculo + " a " + factura.FechaMaximaPago,
                "Llamadas facturadas: " + factura.TotalLlamadas,
                "Monto a cancelar: CRC " + factura.TotalFacturar.ToString("N2"),
                "",
                "Puede ingresar al Portal Cliente para cancelar este cobro.",
                "",
                "Central Telefonica TG");

            return Enviar(
                configuracion,
                factura.CorreoCliente.Trim(),
                "Factura postpago generada - Central Telefonica TG",
                cuerpo,
                "Correo de factura generada enviado.");
        }

        private static EmailEnvioResultado Enviar(
            SmtpConfiguracion configuracion,
            string destinatario,
            string asunto,
            string cuerpo,
            string mensajeExito)
        {
            using (var mensaje = new MailMessage(configuracion.Desde, destinatario))
            {
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;

                using (var cliente = new SmtpClient(configuracion.Host, configuracion.Puerto))
                {
                    cliente.EnableSsl = configuracion.EnableSsl;
                    cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                    cliente.UseDefaultCredentials = false;
                    cliente.Credentials = new NetworkCredential(
                        configuracion.Usuario,
                        configuracion.Clave);

                    try
                    {
                        cliente.Send(mensaje);
                        return new EmailEnvioResultado
                        {
                            Enviado = true,
                            Mensaje = mensajeExito
                        };
                    }
                    catch (Exception ex)
                    {
                        return new EmailEnvioResultado
                        {
                            Enviado = false,
                            Mensaje = "No se pudo enviar el correo: " + ex.Message
                        };
                    }
                }
            }
        }

        private class SmtpConfiguracion
        {
            public string Host { get; set; }

            public int Puerto { get; set; }

            public string Usuario { get; set; }

            public string Clave { get; set; }

            public string Desde { get; set; }

            public bool EnableSsl { get; set; }

            public bool EstaCompleta
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Host)
                        && !string.IsNullOrWhiteSpace(Usuario)
                        && !string.IsNullOrWhiteSpace(Desde)
                        && !string.IsNullOrWhiteSpace(Clave);
                }
            }

            public static SmtpConfiguracion Leer()
            {
                string usuario = LeerValor("SmtpUser", "CentralTelefonica_SmtpUser");
                string desde = LeerValor("SmtpFrom", "CentralTelefonica_SmtpFrom");

                if (string.IsNullOrWhiteSpace(desde))
                {
                    desde = usuario;
                }

                int puerto;
                if (!int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out puerto))
                {
                    puerto = 587;
                }

                bool enableSsl = !string.Equals(
                    ConfigurationManager.AppSettings["SmtpEnableSsl"],
                    "false",
                    StringComparison.OrdinalIgnoreCase);

                return new SmtpConfiguracion
                {
                    Host = LeerValor("SmtpHost", "CentralTelefonica_SmtpHost"),
                    Puerto = puerto,
                    Usuario = usuario,
                    Clave = LeerValor("SmtpPassword", "CentralTelefonica_SmtpPassword"),
                    Desde = desde,
                    EnableSsl = enableSsl
                };
            }

            private static string LeerValor(string appSetting, string variableEntorno)
            {
                string valor = ConfigurationManager.AppSettings[appSetting] ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor;
                }

                return Environment.GetEnvironmentVariable(variableEntorno) ?? string.Empty;
            }
        }
    }
}
