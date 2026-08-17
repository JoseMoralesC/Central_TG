using System;
using System.Collections.Generic;

namespace WebCliente
{
    public partial class Lineas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioCliente"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                PrepagoGrid.DataSource = new List<object>();
                PrepagoGrid.DataBind();
                PostpagoGrid.DataSource = new List<object>();
                PostpagoGrid.DataBind();
            }
        }
    }
}
