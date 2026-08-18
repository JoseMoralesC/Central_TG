using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class LineasActivar : System.Web.UI.Page
    {
        private readonly ProveedorSoapClient _proveedorClient = new ProveedorSoapClient();
        private readonly AutenticacionSoapClient _autenticacionClient = new AutenticacionSoapClient();

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
                CargarClientes();
                CargarLineas();
            }
        }

        protected void LineasGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SeleccionarLinea")
            {
                return;
            }

            string[] partes = Convert.ToString(e.CommandArgument).Split('|');
            if (partes.Length < 5)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            NumeroHidden.Value = partes[1];
            IdentificadorTelefonoHidden.Value = partes[2];
            IdentificadorTarjetaHidden.Value = partes[3];
            TipoHidden.Value = partes[4];

            if (string.IsNullOrWhiteSpace(IdentificadorTelefonoHidden.Value) ||
                string.IsNullOrWhiteSpace(IdentificadorTarjetaHidden.Value))
            {
                ActivacionPanel.Visible = false;
                MensajeLabel.Text = "La linea seleccionada no tiene SIM/IMEI registrados y no se puede asignar.";
                return;
            }

            TelefonoLiteral.Text = partes[1];
            TipoLiteral.Text = partes[4];
            ActivacionPanel.Visible = true;
        }

        protected void ActivarButton_Click(object sender, EventArgs e)
        {
            string identificacionCliente = ClientesList.SelectedValue;

            if (string.IsNullOrWhiteSpace(identificacionCliente) ||
                string.IsNullOrWhiteSpace(NumeroHidden.Value) ||
                string.IsNullOrWhiteSpace(TipoHidden.Value))
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                UsuarioServicio cliente = ObtenerClienteActivo(identificacionCliente);
                if (cliente == null)
                {
                    MensajeLabel.Text = "El cliente indicado no existe o no esta activo.";
                    return;
                }

                var solicitud = new ActivarDesactivarLineaRequest
                {
                    NumeroTelefono = ProveedorCryptoHelper.Encrypt(NumeroHidden.Value),
                    IdentificadorTelefono = IdentificadorTelefonoHidden.Value,
                    IdentificadorTarjeta = IdentificadorTarjetaHidden.Value,
                    Tipo = TipoHidden.Value,
                    IdentificacionCliente = ProveedorCryptoHelper.Encrypt(identificacionCliente),
                    Estado = "activo",
                    NombreCliente = NombreCompleto(cliente),
                    CorreoCliente = cliente.CorreoElectronico
                };

                var respuesta = _proveedorClient.ActivarDesactivarLinea(solicitud);
                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";

                if (respuesta != null && respuesta.Resultado)
                {
                    ActivacionPanel.Visible = false;
                    CargarLineas();
                }
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        private void CargarClientes()
        {
            ClientesList.Items.Clear();
            ClientesList.Items.Add(new ListItem("Seleccione un cliente", string.Empty));

            try
            {
                List<UsuarioServicio> clientes = ObtenerUsuariosMongoAsignables();

                if (clientes.Count == 0)
                {
                    MensajeLabel.Text = "No hay usuarios activos disponibles en MongoDB.";
                    return;
                }

                foreach (UsuarioServicio cliente in clientes)
                {
                    ClientesList.Items.Add(new ListItem(
                        cliente.Identificacion + " - " + NombreCompleto(cliente),
                        cliente.Identificacion));
                }
            }
            catch (Exception)
            {
                MensajeLabel.Text = "No se pudieron consultar clientes activos.";
            }
        }

        private UsuarioServicio ObtenerClienteActivo(string identificacion)
        {
            string cedula = identificacion.Trim();

            return ObtenerUsuariosMongoAsignables().FirstOrDefault(usuario =>
                string.Equals(usuario.Identificacion, cedula, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(usuario.Estado, "activo", StringComparison.OrdinalIgnoreCase));
        }

        private List<UsuarioServicio> ObtenerUsuariosMongoAsignables()
        {
            return UsuariosActivos(
                _autenticacionClient.ListarClientes(),
                _autenticacionClient.ListarAdministradores());
        }

        private static List<UsuarioServicio> UsuariosActivos(params ResultadoListadoUsuarios[] respuestas)
        {
            return respuestas
                .Where(respuesta => respuesta?.Usuarios != null)
                .SelectMany(respuesta => respuesta.Usuarios)
                .Where(usuario => string.Equals(usuario.Estado, "activo", StringComparison.OrdinalIgnoreCase))
                .GroupBy(usuario => usuario.Identificacion)
                .Select(grupo => grupo.First())
                .OrderBy(usuario => usuario.Identificacion)
                .ToList();
        }

        private static string NombreCompleto(UsuarioServicio cliente)
        {
            string[] partes =
            {
                cliente.Nombre,
                cliente.PrimerApellido,
                cliente.SegundoApellido
            };

            return string.Join(" ", partes.Where(parte => !string.IsNullOrWhiteSpace(parte)));
        }

        private void CargarLineas()
        {
            try
            {
                var respuesta = _proveedorClient.ListarLineasDisponibles();
                LineasGrid.DataSource = respuesta?.Lineas ?? Enumerable.Empty<LineaAdministrativaDto>();
                LineasGrid.DataBind();
            }
            catch (Exception)
            {
                LineasGrid.DataSource = Enumerable.Empty<LineaAdministrativaDto>();
                LineasGrid.DataBind();
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }
    }
}
