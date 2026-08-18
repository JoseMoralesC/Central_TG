using System;
using System.Linq;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class LineasDevolucion : System.Web.UI.Page
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
            if (e.CommandName != "DevolverLinea")
            {
                return;
            }

            try
            {
                string[] partes = Convert.ToString(e.CommandArgument).Split('|');
                if (partes.Length < 6)
                {
                    MensajeLabel.Text = "Error al realizar el proceso";
                    return;
                }

                var solicitud = new ActivarDesactivarLineaRequest
                {
                    NumeroTelefono = ProveedorCryptoHelper.Encrypt(partes[1]),
                    IdentificadorTelefono = partes[2],
                    IdentificadorTarjeta = partes[3],
                    Tipo = partes[4],
                    IdentificacionCliente = partes[5],
                    Estado = "disponible"
                };

                var respuesta = _proveedorClient.ActivarDesactivarLinea(solicitud);
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
                var respuesta = _proveedorClient.ListarLineasActivas();
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
