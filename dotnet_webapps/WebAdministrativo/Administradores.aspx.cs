using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class Administradores : System.Web.UI.Page
    {
        private static readonly Regex CorreoRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex ContrasenaRegex =
            new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{14}$", RegexOptions.Compiled);

        private readonly AutenticacionSoapClient _autenticacionClient =
            new AutenticacionSoapClient();

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
                CargarAdministradores();
            }
        }

        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            ModoHidden.Value = "nuevo";
            IdentificacionText.ReadOnly = false;
            UsuarioText.ReadOnly = false;
            FormularioPanel.Visible = true;
        }

        protected void GuardarButton_Click(object sender, EventArgs e)
        {
            if (!FormularioValido())
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                ResultadoOperacion respuesta;

                if (ModoHidden.Value == "editar")
                {
                    respuesta = _autenticacionClient.ModificarAdministradorConUsuarioEncriptado(
                        IdentificacionText.Text,
                        NombreText.Text,
                        PrimerApellidoText.Text,
                        SegundoApellidoText.Text,
                        CorreoText.Text,
                        UsuarioEncriptadoHidden.Value,
                        ContrasenaText.Text);
                }
                else
                {
                    respuesta = _autenticacionClient.CrearAdministrador(
                        IdentificacionText.Text,
                        NombreText.Text,
                        PrimerApellidoText.Text,
                        SegundoApellidoText.Text,
                        CorreoText.Text,
                        UsuarioText.Text,
                        ContrasenaText.Text);
                }

                if (respuesta != null && respuesta.Resultado)
                {
                    MensajeLabel.Text = "Registro exitoso";
                    FormularioPanel.Visible = false;
                    CargarAdministradores();
                    return;
                }

                MensajeLabel.Text = respuesta?.Mensaje ?? "Error al realizar el proceso";
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        protected void AdministradoresGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string argumento = Convert.ToString(e.CommandArgument);

            try
            {
                if (e.CommandName == "EditarAdmin")
                {
                    CargarAdministradorEnFormulario(argumento);
                    return;
                }

                if (e.CommandName == "CambiarEstado")
                {
                    var partes = argumento.Split('|');
                    if (partes.Length != 2)
                    {
                        return;
                    }

                    string nuevoEstado = partes[1] == "activo" ? "inactivo" : "activo";
                    var respuesta = _autenticacionClient.CambiarEstadoAdministrador(partes[0], nuevoEstado);
                    MensajeLabel.Text = respuesta != null && respuesta.Resultado
                        ? "Registro exitoso"
                        : respuesta?.Mensaje ?? "Error al realizar el proceso";
                    CargarAdministradores();
                    return;
                }

                if (e.CommandName == "EliminarAdmin")
                {
                    var respuesta = _autenticacionClient.EliminarAdministrador(argumento);
                    MensajeLabel.Text = respuesta != null && respuesta.Resultado
                        ? "Borrado exitoso"
                        : respuesta?.Mensaje ?? "Error al realizar el proceso";
                    CargarAdministradores();
                }
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private void CargarAdministradores()
        {
            try
            {
                var respuesta = _autenticacionClient.ListarAdministradores();
                var usuarios = respuesta?.Usuarios ?? Enumerable.Empty<UsuarioServicio>();

                foreach (UsuarioServicio usuario in usuarios)
                {
                    usuario.UsuarioVisible = CryptoHelper.DecryptOrSummary(usuario.UsuarioEncriptado);
                    usuario.ContrasenaVisible = "********";
                }

                AdministradoresGrid.DataSource = usuarios;
                AdministradoresGrid.DataBind();
            }
            catch (Exception)
            {
                AdministradoresGrid.DataSource = Enumerable.Empty<UsuarioServicio>();
                AdministradoresGrid.DataBind();
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private void CargarAdministradorEnFormulario(string identificacion)
        {
            var respuesta = _autenticacionClient.ListarAdministradores();
            var usuario = respuesta?.Usuarios?.FirstOrDefault(u => u.Identificacion == identificacion);

            if (usuario == null)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            ModoHidden.Value = "editar";
            IdentificacionText.Text = usuario.Identificacion;
            IdentificacionText.ReadOnly = true;
            NombreText.Text = usuario.Nombre;
            PrimerApellidoText.Text = usuario.PrimerApellido;
            SegundoApellidoText.Text = usuario.SegundoApellido;
            CorreoText.Text = usuario.CorreoElectronico;
            UsuarioEncriptadoHidden.Value = usuario.UsuarioEncriptado;
            UsuarioText.Text = "Sin cambios";
            UsuarioText.ReadOnly = true;
            ContrasenaText.Text = string.Empty;
            FormularioPanel.Visible = true;
        }

        private bool FormularioValido()
        {
            bool editando = ModoHidden.Value == "editar";
            bool contrasenaValida = editando
                ? string.IsNullOrWhiteSpace(ContrasenaText.Text) || ContrasenaRegex.IsMatch(ContrasenaText.Text)
                : ContrasenaRegex.IsMatch(ContrasenaText.Text);

            return !string.IsNullOrWhiteSpace(IdentificacionText.Text) &&
                !string.IsNullOrWhiteSpace(NombreText.Text) &&
                !string.IsNullOrWhiteSpace(PrimerApellidoText.Text) &&
                !string.IsNullOrWhiteSpace(CorreoText.Text) &&
                (editando || !string.IsNullOrWhiteSpace(UsuarioText.Text)) &&
                CorreoRegex.IsMatch(CorreoText.Text.Trim()) &&
                contrasenaValida;
        }

        private void LimpiarFormulario()
        {
            IdentificacionText.Text = string.Empty;
            NombreText.Text = string.Empty;
            PrimerApellidoText.Text = string.Empty;
            SegundoApellidoText.Text = string.Empty;
            CorreoText.Text = string.Empty;
            UsuarioText.Text = string.Empty;
            UsuarioText.ReadOnly = false;
            UsuarioEncriptadoHidden.Value = string.Empty;
            ContrasenaText.Text = string.Empty;
            MensajeLabel.Text = string.Empty;
        }
    }
}
