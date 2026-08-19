using System;
using System.Linq;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class SolicitudesLineas : System.Web.UI.Page
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
                CargarSolicitudes();
            }
        }

        protected void SolicitudesGrid_RowCommand(
            object sender,
            System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AsignarSolicitud")
            {
                AsignarSolicitud(Convert.ToString(e.CommandArgument));
                return;
            }

            if (e.CommandName == "RechazarSolicitud")
            {
                RechazarSolicitud(Convert.ToString(e.CommandArgument));
            }
        }

        private void AsignarSolicitud(string argumento)
        {
            string[] partes = (argumento ?? string.Empty).Split('|');
            if (partes.Length < 3 ||
                !int.TryParse(partes[0], out int solicitudId) ||
                !int.TryParse(partes[1], out int servicioId))
            {
                MensajeLabel.Text = "Solicitud incorrecta.";
                return;
            }

            string identificacionCliente = partes[2];

            try
            {
                UsuarioServicio cliente = ObtenerClienteActivo(identificacionCliente);
                if (cliente == null)
                {
                    MensajeLabel.Text = "El cliente indicado no existe o no esta activo.";
                    return;
                }

                LineaAdministrativaDto linea = ObtenerLineaDisponible(servicioId);
                if (linea == null)
                {
                    MensajeLabel.Text = "La linea solicitada ya no esta disponible.";
                    CargarSolicitudes();
                    return;
                }

                var solicitud = new ActivarDesactivarLineaRequest
                {
                    NumeroTelefono = ProveedorCryptoHelper.Encrypt(linea.NumeroTelefono),
                    IdentificadorTelefono = linea.IdentificadorTelefono,
                    IdentificadorTarjeta = linea.IdentificadorTarjeta,
                    Tipo = linea.TipoServicio,
                    IdentificacionCliente = ProveedorCryptoHelper.Encrypt(identificacionCliente),
                    Estado = "activo",
                    NombreCliente = NombreCompleto(cliente),
                    CorreoCliente = cliente.CorreoElectronico
                };

                RespuestaServicio respuesta = _proveedorClient.ActivarDesactivarLinea(solicitud);
                if (respuesta == null || !respuesta.Resultado)
                {
                    MensajeLabel.Text = respuesta?.Mensaje ?? "Error al asignar la linea.";
                    return;
                }

                RespuestaServicio atendida =
                    _proveedorClient.MarcarSolicitudLineaAtendida(solicitudId, "APROBADA");

                MensajeLabel.Text = atendida != null && atendida.Resultado
                    ? "Linea asignada y solicitud aprobada."
                    : "Linea asignada, pero no se pudo actualizar la solicitud.";

                CargarSolicitudes();
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al asignar la solicitud.";
            }
        }

        private void RechazarSolicitud(string argumento)
        {
            if (!int.TryParse(argumento, out int solicitudId))
            {
                MensajeLabel.Text = "Solicitud incorrecta.";
                return;
            }

            try
            {
                RespuestaServicio respuesta =
                    _proveedorClient.MarcarSolicitudLineaAtendida(solicitudId, "RECHAZADA");

                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Solicitud rechazada."
                    : respuesta?.Mensaje ?? "No se pudo rechazar la solicitud.";

                CargarSolicitudes();
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al rechazar la solicitud.";
            }
        }

        private void CargarSolicitudes()
        {
            try
            {
                ListadoSolicitudesLineaResponse respuesta =
                    _proveedorClient.ListarSolicitudesLineaPendientes();

                SolicitudesGrid.DataSource =
                    respuesta?.Solicitudes ?? Enumerable.Empty<SolicitudLineaDto>();
                SolicitudesGrid.DataBind();

                if (respuesta != null &&
                    respuesta.Resultado &&
                    (respuesta.Solicitudes == null || respuesta.Solicitudes.Length == 0))
                {
                    MensajeLabel.Text = respuesta.Mensaje;
                }
            }
            catch (Exception)
            {
                SolicitudesGrid.DataSource = Enumerable.Empty<SolicitudLineaDto>();
                SolicitudesGrid.DataBind();
                MensajeLabel.Text = "No se pudieron consultar las solicitudes.";
            }
        }

        private LineaAdministrativaDto ObtenerLineaDisponible(int servicioId)
        {
            return _proveedorClient.ListarLineasDisponibles()
                ?.Lineas
                ?.FirstOrDefault(linea => linea.ServicioId == servicioId);
        }

        private UsuarioServicio ObtenerClienteActivo(string identificacion)
        {
            string cedula = identificacion.Trim();

            return _autenticacionClient.ListarClientes()
                ?.Usuarios
                ?.FirstOrDefault(usuario =>
                    string.Equals(usuario.Identificacion, cedula, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(usuario.Estado, "activo", StringComparison.OrdinalIgnoreCase));
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
    }
}
