using System;
using WebCliente.Services;

namespace WebCliente
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly AutenticacionSoapClient _autenticacionClient =
            new AutenticacionSoapClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }

        protected void IngresarButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsuarioText.Text) ||
                string.IsNullOrWhiteSpace(ContrasenaText.Text))
            {
                MensajeLabel.Text = "Usuario y/o contrasena incorrectos";
                return;
            }

            try
            {
                var respuesta = _autenticacionClient.LoginCliente(
                    UsuarioText.Text,
                    ContrasenaText.Text);

                if (respuesta == null ||
                    !respuesta.Resultado ||
                    respuesta.Usuario == null)
                {
                    MensajeLabel.Text = "Usuario y/o contrasena incorrectos";
                    return;
                }

                Session["UsuarioCliente"] = UsuarioText.Text.Trim();
                Session["IdentificacionCliente"] = respuesta.Usuario.Identificacion;
                Session["NombreCliente"] = respuesta.Usuario.Nombre;
                Response.Redirect("~/Lineas.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Usuario y/o contrasena incorrectos";
            }
        }
    }
}
