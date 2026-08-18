using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WebAdministrativo.Services;

namespace WebAdministrativo
{
    public partial class LineasNuevas : System.Web.UI.Page
    {
        private readonly ProveedorSoapClient _proveedorClient = new ProveedorSoapClient();

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
                CargarLineas();
            }
        }

        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            GenerarIdentificadoresRegistro();
            FormularioPanel.Visible = true;
            MensajeLabel.Text = string.Empty;
        }

        protected void GenerarIdsButton_Click(object sender, EventArgs e)
        {
            GenerarIdentificadoresRegistro();
            FormularioPanel.Visible = true;
            MensajeLabel.Text = string.Empty;
        }

        protected void GuardarButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdentificadorTelefonoText.Text) ||
                string.IsNullOrWhiteSpace(IdentificadorTarjetaText.Text))
            {
                GenerarIdentificadoresRegistro();
            }

            if (!FormularioValido())
            {
                MensajeLabel.Text = "Error al realizar el proceso";
                return;
            }

            try
            {
                var respuesta = _proveedorClient.RegistrarLinea(
                    NumeroText.Text,
                    IdentificadorTelefonoText.Text,
                    IdentificadorTarjetaText.Text,
                    TipoServicioList.SelectedValue);

                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";

                if (respuesta != null && respuesta.Resultado)
                {
                    LimpiarFormulario();
                    FormularioPanel.Visible = false;
                    CargarLineas();
                }
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
        }

        protected void LineasGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EliminarLinea")
            {
                return;
            }

            try
            {
                int servicioId = Convert.ToInt32(e.CommandArgument);
                var respuesta = _proveedorClient.EliminarLineaDisponible(servicioId);
                MensajeLabel.Text = respuesta != null && respuesta.Resultado
                    ? "Proceso finalizado de forma exitosa"
                    : respuesta?.Mensaje ?? "Error al realizar el proceso";
                CargarLineas();
            }
            catch (Exception)
            {
                MensajeLabel.Text = "Error al realizar el proceso";
            }
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

        private bool FormularioValido()
        {
            return !string.IsNullOrWhiteSpace(NumeroText.Text) &&
                !string.IsNullOrWhiteSpace(IdentificadorTelefonoText.Text) &&
                !string.IsNullOrWhiteSpace(IdentificadorTarjetaText.Text) &&
                (TipoServicioList.SelectedValue == "PREPAGO" ||
                 TipoServicioList.SelectedValue == "POSTPAGO");
        }

        private void LimpiarFormulario()
        {
            NumeroText.Text = string.Empty;
            IdentificadorTelefonoText.Text = string.Empty;
            IdentificadorTarjetaText.Text = string.Empty;
        }

        private void GenerarIdentificadoresRegistro()
        {
            List<LineaAdministrativaDto> lineas = ObtenerLineasExistentes();

            string sim;
            do
            {
                sim = "ENC_SIM_" + GenerarDigitos(19);
            }
            while (ExisteIdentificadorTarjeta(lineas, sim));

            string imei;
            do
            {
                imei = "ENC_IMEI_" + GenerarDigitos(16);
            }
            while (ExisteIdentificadorTelefono(lineas, imei));

            IdentificadorTarjetaText.Text = sim;
            IdentificadorTelefonoText.Text = imei;
        }

        private List<LineaAdministrativaDto> ObtenerLineasExistentes()
        {
            var lineas = new List<LineaAdministrativaDto>();

            try
            {
                lineas.AddRange(_proveedorClient.ListarLineasDisponibles()?.Lineas ??
                    Array.Empty<LineaAdministrativaDto>());
            }
            catch
            {
            }

            try
            {
                lineas.AddRange(_proveedorClient.ListarLineasActivas()?.Lineas ??
                    Array.Empty<LineaAdministrativaDto>());
            }
            catch
            {
            }

            return lineas;
        }

        private static bool ExisteIdentificadorTarjeta(
            IEnumerable<LineaAdministrativaDto> lineas,
            string identificador)
        {
            return lineas.Any(linea =>
                IdentificadoresIguales(linea.IdentificadorTarjeta, identificador) ||
                IdentificadoresIguales(linea.IdentificadorTarjetaVisible, identificador));
        }

        private static bool ExisteIdentificadorTelefono(
            IEnumerable<LineaAdministrativaDto> lineas,
            string identificador)
        {
            return lineas.Any(linea =>
                IdentificadoresIguales(linea.IdentificadorTelefono, identificador) ||
                IdentificadoresIguales(linea.IdentificadorTelefonoVisible, identificador));
        }

        private static bool IdentificadoresIguales(string valorActual, string valorNuevo)
        {
            return string.Equals(
                valorActual?.Trim(),
                valorNuevo?.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GenerarDigitos(int longitud)
        {
            byte[] buffer = new byte[longitud];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }

            char[] digitos = new char[longitud];

            for (int i = 0; i < buffer.Length; i++)
            {
                digitos[i] = (char)('0' + (buffer[i] % 10));
            }

            return new string(digitos);
        }
    }
}
