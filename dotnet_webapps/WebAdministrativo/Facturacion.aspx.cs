using System;
using System.Linq;
using System.Web.UI.WebControls;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class Facturacion : System.Web.UI.Page
    {
        private readonly ProveedorSoapClient _proveedorClient =
            new ProveedorSoapClient();
        private readonly EmailService _emailService = new EmailService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioAdmin"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                CargarUltimaFacturacion();
                CargarLineasPostpago();
                SugerirFechasCalculo();
            }
        }

        protected void CalcularButton_Click(object sender, EventArgs e)
        {
            LimpiarConsultaPendiente();

            if (!DateTime.TryParse(FechaCalculoText.Text, out var fechaCalculo) ||
                !DateTime.TryParse(FechaMaximaPagoText.Text, out var fechaMaximaPago))
            {
                MensajeLabel.Text = "Ingrese fechas validas para calcular la facturacion.";
                return;
            }

            if (fechaMaximaPago < fechaCalculo)
            {
                MensajeLabel.Text = "La fecha maxima de pago no puede ser anterior a la fecha de calculo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(LineaPostpagoList.SelectedValue))
            {
                MensajeLabel.Text = "Seleccione una linea postpago activa para consultar.";
                return;
            }

            try
            {
                var respuesta = _proveedorClient.ConsultarFacturacion(
                    fechaCalculo,
                    fechaMaximaPago,
                    LineaPostpagoList.SelectedValue);

                if (respuesta != null && respuesta.Resultado)
                {
                    MensajeLabel.Text = respuesta.Mensaje;
                    GuardarConsultaPendiente(respuesta);
                    return;
                }

                MensajeLabel.Text = respuesta?.Mensaje ?? "Error al realizar el proceso";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        protected void GenerarFacturaButton_Click(object sender, EventArgs e)
        {
            if (!TieneConsultaPendiente())
            {
                MensajeLabel.Text = "Primero consulte la factura para poder generarla.";
                return;
            }

            string numeroTelefono = Convert.ToString(ViewState["ConsultaNumeroTelefono"]);
            string fechaCalculoTexto = Convert.ToString(ViewState["ConsultaFechaCalculo"]);
            string fechaMaximaPagoTexto = Convert.ToString(ViewState["ConsultaFechaMaximaPago"]);

            if (!ConsultaCoincideConFormulario(
                numeroTelefono,
                fechaCalculoTexto,
                fechaMaximaPagoTexto))
            {
                MensajeLabel.Text = "La linea o las fechas cambiaron. Vuelva a consultar antes de generar la factura.";
                LimpiarConsultaPendiente();
                return;
            }

            if (!DateTime.TryParse(fechaCalculoTexto, out var fechaCalculo) ||
                !DateTime.TryParse(fechaMaximaPagoTexto, out var fechaMaximaPago))
            {
                MensajeLabel.Text = "La consulta previa perdio sus fechas. Vuelva a consultar.";
                LimpiarConsultaPendiente();
                return;
            }

            decimal totalFacturar = Convert.ToDecimal(ViewState["ConsultaTotalFacturar"] ?? 0m);
            int totalLlamadas = Convert.ToInt32(ViewState["ConsultaTotalLlamadas"] ?? 0);

            if (totalFacturar <= 0m || totalLlamadas <= 0)
            {
                MensajeLabel.Text = "La consulta no posee consumo facturable. No se puede generar una factura en cero.";
                GenerarFacturaButton.Enabled = false;
                return;
            }

            try
            {
                var respuesta = _proveedorClient.CalcularFacturacion(
                    fechaCalculo,
                    fechaMaximaPago,
                    numeroTelefono);

                if (respuesta != null && respuesta.Resultado)
                {
                    var envioCorreo = _emailService.EnviarFacturaGenerada(
                        new FacturaGeneradaEmail
                        {
                            CorreoCliente = Convert.ToString(ViewState["ConsultaCorreoCliente"]),
                            NombreCliente = Convert.ToString(ViewState["ConsultaNombreCliente"]),
                            NumeroTelefono = numeroTelefono,
                            FechaCalculo = fechaCalculoTexto,
                            FechaMaximaPago = fechaMaximaPagoTexto,
                            TotalLlamadas = totalLlamadas,
                            TotalFacturar = totalFacturar
                        });

                    MensajeLabel.Text = respuesta.Mensaje + " " + envioCorreo.Mensaje;
                    LimpiarConsultaPendiente();
                    CargarUltimaFacturacion();
                    return;
                }

                MensajeLabel.Text = respuesta?.Mensaje ?? "Error al generar la factura.";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al generar la factura.";
            }
        }

        private void CargarLineasPostpago()
        {
            try
            {
                var lineas = (_proveedorClient.ListarLineasActivas()?.Lineas ??
                    Enumerable.Empty<LineaAdministrativaDto>())
                    .Where(linea => string.Equals(
                        linea.TipoServicio,
                        "POSTPAGO",
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                LineaPostpagoList.Items.Clear();

                foreach (var linea in lineas)
                {
                    string texto = linea.NumeroTelefono;

                    if (!string.IsNullOrWhiteSpace(linea.NombreCliente))
                    {
                        texto += " - " + linea.NombreCliente;
                    }

                    LineaPostpagoList.Items.Add(new ListItem(texto, linea.NumeroTelefono));
                    ViewState["LineaCorreo_" + linea.NumeroTelefono] = linea.CorreoCliente;
                    ViewState["LineaNombre_" + linea.NumeroTelefono] = linea.NombreCliente;
                }

                if (LineaPostpagoList.Items.Count == 0)
                {
                    LineaPostpagoList.Items.Add(new ListItem(
                        "No hay lineas postpago activas",
                        string.Empty));
                }
            }
            catch (Exception)
            {
                LineaPostpagoList.Items.Clear();
                LineaPostpagoList.Items.Add(new ListItem(
                    "Error cargando lineas postpago",
                    string.Empty));
            }
        }

        private void CargarUltimaFacturacion()
        {
            try
            {
                var respuesta = _proveedorClient.ObtenerUltimaFacturacion();

                if (respuesta == null || !respuesta.Resultado)
                {
                    UltimaFacturacionLabel.Text = "Error al consultar el ultimo calculo.";
                    ViewState["UltimaFechaCalculo"] = null;
                    return;
                }

                if (!respuesta.HayFacturacion)
                {
                    UltimaFacturacionLabel.Text = "No existe calculo previo.";
                    ViewState["UltimaFechaCalculo"] = null;
                    return;
                }

                UltimaFacturacionLabel.Text =
                    "Linea: " + respuesta.NumeroTelefono +
                    " | Fecha calculo: " + respuesta.FechaCalculo +
                    " | Fecha maxima pago: " + respuesta.FechaMaximaPago +
                    " | Llamadas: " + respuesta.TotalLlamadas +
                    " | Total: " + respuesta.TotalFacturar.ToString("0.00");

                if (DateTime.TryParse(respuesta.FechaCalculo, out var ultimaFecha))
                {
                    ViewState["UltimaFechaCalculo"] = ultimaFecha;
                }
            }
            catch (Exception)
            {
                UltimaFacturacionLabel.Text = "Error al consultar el ultimo calculo.";
                ViewState["UltimaFechaCalculo"] = null;
            }
        }

        private void SugerirFechasCalculo()
        {
            DateTime fechaCalculo = DateTime.Today;

            FechaCalculoText.Text = fechaCalculo.ToString("yyyy-MM-dd");
            FechaMaximaPagoText.Text = new DateTime(
                fechaCalculo.Year,
                fechaCalculo.Month,
                DateTime.DaysInMonth(fechaCalculo.Year, fechaCalculo.Month))
                .ToString("yyyy-MM-dd");
        }

        private void GuardarConsultaPendiente(FacturacionConsultaResponse respuesta)
        {
            ViewState["ConsultaNumeroTelefono"] = respuesta.NumeroTelefono;
            ViewState["ConsultaFechaCalculo"] = respuesta.FechaCalculo;
            ViewState["ConsultaFechaMaximaPago"] = respuesta.FechaMaximaPago;
            ViewState["ConsultaTotalLlamadas"] = respuesta.TotalLlamadas;
            ViewState["ConsultaTotalFacturar"] = respuesta.TotalFacturar;
            string claveLinea = string.IsNullOrWhiteSpace(LineaPostpagoList.SelectedValue)
                ? respuesta.NumeroTelefono
                : LineaPostpagoList.SelectedValue;

            ViewState["ConsultaCorreoCliente"] = ObtenerDatoLinea(
                respuesta.NumeroTelefono,
                claveLinea,
                "Correo");
            ViewState["ConsultaNombreCliente"] = ObtenerDatoLinea(
                respuesta.NumeroTelefono,
                claveLinea,
                "Nombre");

            ResultadoConsultaPanel.Visible = true;
            GenerarFacturaButton.Enabled =
                respuesta.TotalFacturar > 0m && respuesta.TotalLlamadas > 0;
            ResultadoConsultaLabel.Text =
                " Linea: " + respuesta.NumeroTelefono +
                " | Periodo: " + respuesta.FechaCalculo + " a " + respuesta.FechaMaximaPago +
                " | Llamadas: " + respuesta.TotalLlamadas +
                " | Total a facturar: " + respuesta.TotalFacturar.ToString("0.00") + " CRC" +
                (GenerarFacturaButton.Enabled
                    ? string.Empty
                    : " | Sin consumo para generar factura.");
        }

        private bool TieneConsultaPendiente()
        {
            return !string.IsNullOrWhiteSpace(
                Convert.ToString(ViewState["ConsultaNumeroTelefono"]));
        }

        private bool ConsultaCoincideConFormulario(
            string numeroTelefono,
            string fechaCalculo,
            string fechaMaximaPago)
        {
            return string.Equals(
                    numeroTelefono?.Trim(),
                    LineaPostpagoList.SelectedValue?.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fechaCalculo, FechaCalculoText.Text, StringComparison.Ordinal) &&
                string.Equals(fechaMaximaPago, FechaMaximaPagoText.Text, StringComparison.Ordinal);
        }

        private void LimpiarConsultaPendiente()
        {
            ViewState["ConsultaNumeroTelefono"] = null;
            ViewState["ConsultaFechaCalculo"] = null;
            ViewState["ConsultaFechaMaximaPago"] = null;
            ViewState["ConsultaTotalLlamadas"] = null;
            ViewState["ConsultaTotalFacturar"] = null;
            ViewState["ConsultaCorreoCliente"] = null;
            ViewState["ConsultaNombreCliente"] = null;
            ResultadoConsultaPanel.Visible = false;
            ResultadoConsultaLabel.Text = string.Empty;
            GenerarFacturaButton.Enabled = true;
        }

        private string ObtenerDatoLinea(string numeroRespuesta, string numeroSeleccionado, string campo)
        {
            string valor = Convert.ToString(ViewState["Linea" + campo + "_" + numeroRespuesta]);

            if (string.IsNullOrWhiteSpace(valor) &&
                !string.Equals(numeroRespuesta, numeroSeleccionado, StringComparison.Ordinal))
            {
                valor = Convert.ToString(ViewState["Linea" + campo + "_" + numeroSeleccionado]);
            }

            return valor;
        }
    }
}
