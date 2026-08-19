using System;
using System.Configuration;
using System.Web;

namespace WebCliente
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string nombre = Session["NombreCliente"] as string;
            SaludoLabel.Text = string.IsNullOrWhiteSpace(nombre) ? string.Empty : "Hola " + nombre;
            ConfigurarLinksPortal();
        }

        protected void SalirLink_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ConfigurarLinksPortal()
        {
            string baseUrl = ConfigurationManager.AppSettings["PortalClienteUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return;
            }

            string identificacion = Session["IdentificacionCliente"] as string;
            string separador = baseUrl.Contains("?") ? "&" : "?";
            string url = baseUrl + separador + "identificacion=" + HttpUtility.UrlEncode(identificacion ?? string.Empty);

            LineasLink.NavigateUrl = url;
            RecargaLink.NavigateUrl = url.Replace("/Index", "/CargarSaldo");
            PagoLink.NavigateUrl = url.Replace("/Index", "/PagarFactura");
            DevolucionLink.NavigateUrl = url.Replace("/Index", "/DevolverLinea");
        }
    }
}
