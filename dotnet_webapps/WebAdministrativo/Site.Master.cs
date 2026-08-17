using System;

namespace WebAdministrativo
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void SalirLink_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
