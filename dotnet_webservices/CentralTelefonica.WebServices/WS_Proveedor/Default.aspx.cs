using System;

namespace WS_Proveedor
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/ProveedorService.svc");
        }
    }
}
