using System;
using System.Linq;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class LineasNuevas : System.Web.UI.Page
    {
        private readonly ProveedorSoapClient _proveedorClient = new ProveedorSoapClient();

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
                CargarLineas();
            }
        }

        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            FormularioPanel.Visible = true;
            MensajeLabel.Text = string.Empty;
        }

        protected void GuardarButton_Click(object sender, EventArgs e)
        {
            if (!FormularioValido())
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                var respuesta = _proveedorClient.RegistrarLinea(
                    NumeroText.Text,
                    IdentificadorTelefonoText.Text,
                    IdentificadorTarjetaText.Text,
                    TipoServicioList.SelectedValue);

                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";

                if (respuesta != null && respuesta.Resultado)
                {
                    LimpiarFormulario();
                    FormularioPanel.Visible = false;
                    CargarLineas();
                }
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        protected void LineasGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EliminarLinea")
            {
                return;
            }

            try
            {
                int servicioId = Convert.ToInt32(e.CommandArgument);
                var respuesta = _proveedorClient.EliminarLineaDisponible(servicioId);
                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";
                CargarLineas();
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private void CargarLineas()
        {
            try
            {
                var respuesta = _proveedorClient.ListarLineasDisponibles();
                LineasGrid.DataSource = respuesta?.Lineas ?? Enumerable.Empty<LineaAdministrativaDto>();
                LineasGrid.DataBind();
            }
            catch (Exception)
            {
                LineasGrid.DataSource = Enumerable.Empty<LineaAdministrativaDto>();
                LineasGrid.DataBind();
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private bool FormularioValido()
        {
            return !string.IsNullOrWhiteSpace(NumeroText.Text) &&
                !string.IsNullOrWhiteSpace(IdentificadorTelefonoText.Text) &&
                !string.IsNullOrWhiteSpace(IdentificadorTarjetaText.Text) &&
                (TipoServicioList.SelectedValue == "PREPAGO" ||
                 TipoServicioList.SelectedValue == "POSTPAGO");
        }

        private void LimpiarFormulario()
        {
            NumeroText.Text = string.Empty;
            IdentificadorTelefonoText.Text = string.Empty;
            IdentificadorTarjetaText.Text = string.Empty;
        }
    }
}
