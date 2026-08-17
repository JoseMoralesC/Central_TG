using System;

namespace WebCliente
{
    public partial class Portal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioCliente"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string operacion = (Request.QueryString["op"] ?? string.Empty).ToLowerInvariant();

            if (operacion == "recarga")
            {
                TituloLiteral.Text = "Cargar saldo a linea prepago";
            }
            else if (operacion == "pago")
            {
                TituloLiteral.Text = "Pagar facturas pendientes de pago";
            }
            else if (operacion == "devolucion")
            {
                TituloLiteral.Text = "Devolver una linea";
            }
            else
            {
                TituloLiteral.Text = "Portal cliente";
            }

            MensajeLabel.Text = "Pantalla reservada para integracion con las historias CLIENTE4 a CLIENTE7.";
        }
    }
}
