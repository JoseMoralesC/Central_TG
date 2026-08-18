using System;
using System.Linq;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class LineasActivar : System.Web.UI.Page
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

        protected void LineasGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SeleccionarLinea")
            {
                return;
            }

            string[] partes = Convert.ToString(e.CommandArgument).Split('|');
            if (partes.Length < 5)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            NumeroHidden.Value = partes[1];
            IdentificadorTelefonoHidden.Value = partes[2];
            IdentificadorTarjetaHidden.Value = partes[3];
            TipoHidden.Value = partes[4];
            TelefonoLiteral.Text = partes[1];
            TipoLiteral.Text = partes[4];
            CedulaText.Text = string.Empty;
            ActivacionPanel.Visible = true;
        }

        protected void ActivarButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CedulaText.Text) ||
                string.IsNullOrWhiteSpace(NumeroHidden.Value) ||
                string.IsNullOrWhiteSpace(TipoHidden.Value))
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                var solicitud = new ActivarDesactivarLineaRequest
                {
                    NumeroTelefono = ProveedorCryptoHelper.Encrypt(NumeroHidden.Value),
                    IdentificadorTelefono = IdentificadorTelefonoHidden.Value,
                    IdentificadorTarjeta = IdentificadorTarjetaHidden.Value,
                    Tipo = TipoHidden.Value,
                    IdentificacionCliente = ProveedorCryptoHelper.Encrypt(CedulaText.Text),
                    Estado = "activo"
                };

                var respuesta = _proveedorClient.ActivarDesactivarLinea(solicitud);
                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";

                if (respuesta != null && respuesta.Resultado)
                {
                    ActivacionPanel.Visible = false;
                    CargarLineas();
                }
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
    }
}
