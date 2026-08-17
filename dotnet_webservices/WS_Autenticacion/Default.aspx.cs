using System;

namespace CentralTelefonica.WS_Autenticacion
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Service1.svc");
        }
    }
}
