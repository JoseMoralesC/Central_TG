using System;
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
            }
        }

        protected void CalcularButton_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(FechaCalculoText.Text, out var fechaCalculo) ||
                !DateTime.TryParse(FechaMaximaPagoText.Text, out var fechaMaximaPago) ||
                fechaMaximaPago < fechaCalculo ||
                !FechaContinuaValida(fechaCalculo))
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                var respuesta = _proveedorClient.CalcularFacturacion(fechaCalculo, fechaMaximaPago);

                if (respuesta != null && respuesta.Resultado)
                {
                    MensajeLabel.Text = "Proceso finalizado de forma exitosa";
                    CargarUltimaFacturacion();
                    return;
                }

                MensajeLabel.Text = "Error al realizar el proceso";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private void CargarUltimaFacturacion()
        {
            try
            {
                var respuesta = _proveedorClient.ObtenerUltimaFacturacion();

                if (respuesta == null || !respuesta.Resultado)
                {
                    UltimaFacturacionLabel.Text = "Error al consultar la ultima facturacion.";
                    ViewState["UltimaFechaCalculo"] = null;
                    return;
                }

                if (!respuesta.HayFacturacion)
                {
                    UltimaFacturacionLabel.Text = "No existe facturacion previa.";
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
                UltimaFacturacionLabel.Text = "Error al consultar la ultima facturacion.";
                ViewState["UltimaFechaCalculo"] = null;
            }
        }

        private bool FechaContinuaValida(DateTime nuevaFechaCalculo)
        {
            if (!(ViewState["UltimaFechaCalculo"] is DateTime ultimaFecha))
            {
                return true;
            }

            return nuevaFechaCalculo.Date == ultimaFecha.Date.AddDays(1);
        }
    }
}
