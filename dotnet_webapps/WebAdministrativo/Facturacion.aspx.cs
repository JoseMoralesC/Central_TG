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
                var respuesta = _proveedorClient.CalcularFacturacion(
                    fechaCalculo,
                    fechaMaximaPago,
                    LineaPostpagoList.SelectedValue);

                if (respuesta != null && respuesta.Resultado)
                {
                    MensajeLabel.Text = respuesta.Mensaje;
                    CargarUltimaFacturacion();
                    return;
                }

                MensajeLabel.Text = respuesta?.Mensaje ?? "Error al realizar el proceso";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
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
                    "Fecha calculo: " + respuesta.FechaCalculo +
                    " | Fecha maxima pago: " + respuesta.FechaMaximaPago +
                    " | Lineas: " + respuesta.TotalLineas +
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
    }
}
