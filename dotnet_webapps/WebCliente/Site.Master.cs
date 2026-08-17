using System;

namespace WebCliente
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string nombre = Session["NombreCliente"] as string;
            SaludoLabel.Text = string.IsNullOrWhiteSpace(nombre) ? string.Empty : "Hola " + nombre;
        }

        protected void SalirLink_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
