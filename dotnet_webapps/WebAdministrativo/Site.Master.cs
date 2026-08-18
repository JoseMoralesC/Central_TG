using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

namespace WebAdministrativo
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MarcarMenuActivo();
        }

        protected void SalirLink_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void MarcarMenuActivo()
        {
            string paginaActual = Path.GetFileName(Request.Path);

            var enlaces = new Dictionary<string, HyperLink>
            {
                { "LineasNuevas.aspx", LineasNuevasLink },
                { "LineasActivar.aspx", LineasActivarLink },
                { "LineasDevolucion.aspx", LineasDevolucionLink },
                { "Facturacion.aspx", FacturacionLink },
                { "Administradores.aspx", AdministradoresLink }
            };

            foreach (var enlace in enlaces)
            {
                enlace.Value.CssClass = string.Equals(
                    paginaActual,
                    enlace.Key,
                    StringComparison.OrdinalIgnoreCase)
                    ? "active"
                    : string.Empty;
            }
        }
    }
}
