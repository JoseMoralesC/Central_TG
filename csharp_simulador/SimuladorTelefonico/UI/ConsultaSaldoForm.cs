using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;
using SimuladorTelefonico.Services;
using SimuladorTelefonico.Utils;

namespace SimuladorTelefonico.UI
{
    public class ConsultaSaldoForm : Form
    {
        private TextBox txtCodigoConsulta = null!;
        private Label lblEstado = null!;
        private Label lblSaldo = null!;
        private Button btnConsultar = null!;
        private Button btnVolver = null!;

        private readonly TramaService _tramaService = new();
        private readonly RespuestaService _respuestaService = new();
        private readonly CryptoService _cryptoService = new();

        public ConsultaSaldoForm()
        {
            ConfigurarVentana();
            ConstruirFormulario();
        }

        private void ConfigurarVentana()
        {
            Text = "Consultar saldo";
            UiTheme.ConfigurarVentana(this, new Size(430, 500));
        }

        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Consulta de saldo",
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 76
            };

            Label lblTelefono = UiTheme.CrearEtiqueta(
                $"Telefono: {AppConfig.NumeroOrigen}",
                40,
                82,
                350,
                24,
                9.5f,
                FontStyle.Bold,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            Label lblCodigo = UiTheme.CrearEtiqueta(
                "Codigo de consulta",
                40,
                128,
                350,
                24,
                10.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            txtCodigoConsulta = new TextBox
            {
                Font = new Font("Segoe UI", 18),
                TextAlign = HorizontalAlignment.Center,
                Top = 158,
                Left = 70,
                Width = 290,
                Height = 40,
                Text = "#9090*",
                MaxLength = 6
            };

            lblSaldo = UiTheme.CrearEtiqueta(
                "Saldo pendiente de consulta",
                40,
                220,
                350,
                42,
                15,
                FontStyle.Bold,
                UiTheme.Texto,
                ContentAlignment.MiddleCenter);

            btnConsultar = CrearBoton("Consultar saldo", 286, UiTheme.Primario);
            btnVolver = CrearBoton("Volver", 338, Color.FromArgb(52, 60, 72));

            lblEstado = UiTheme.CrearEtiqueta(
                "Listo para consultar.",
                40,
                402,
                350,
                38,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            btnConsultar.Click += async (s, e) => await ConsultarSaldoAsync();
            btnVolver.Click += (s, e) => Close();

            Controls.Add(titulo);
            Controls.Add(lblTelefono);
            Controls.Add(lblCodigo);
            Controls.Add(txtCodigoConsulta);
            Controls.Add(lblSaldo);
            Controls.Add(btnConsultar);
            Controls.Add(btnVolver);
            Controls.Add(lblEstado);
        }

        private Button CrearBoton(string texto, int top, Color color)
        {
            return UiTheme.CrearBoton(texto, 70, top, 290, 42, color);
        }

        private async Task ConsultarSaldoAsync()
        {
            string codigoConsulta = txtCodigoConsulta.Text.Trim();

            if (!ValidacionTelefono.EsCodigoConsultaSaldo(codigoConsulta))
            {
                MessageBox.Show(
                    "Para consultar saldo debe ingresar exactamente el codigo #9090*.",
                    "Codigo invalido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            btnConsultar.Enabled = false;
            lblEstado.Text = "Consultando saldo con el Identificador...";
            lblSaldo.Text = "Procesando...";
            lblSaldo.ForeColor = UiTheme.Texto;

            ConsultaSaldo consulta = new ConsultaSaldo
            {
                TelefonoOrigen =
                    _cryptoService.CifrarDatoSensible(AppConfig.NumeroOrigen),

                IdentificadorTelefono =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTelefono),

                IdentificadorDispositivo =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorDispositivo),

                IdentificadorTarjeta =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTarjeta),

                Ubicacion = new UbicacionTelefono
                {
                    Pais = AppConfig.Pais,
                    Provincia = AppConfig.Provincia,
                    Latitud = AppConfig.Latitud,
                    Longitud = AppConfig.Longitud
                },

                FechaHora =
                    DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string respuesta = await _tramaService.EnviarTramaAsync(consulta);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            if (_respuestaService.EsRespuestaExitosa(respuesta))
            {
                if (_respuestaService.TryObtenerSaldoDecimal(respuesta, out decimal saldoActualizado))
                {
                    AppConfig.ActualizarSaldoTelefonoActual(saldoActualizado);
                }

                lblEstado.Text = $"{_respuestaService.ObtenerCodigo(respuesta)} - {mensaje}";
                lblSaldo.Text = _respuestaService.ObtenerSaldoTexto(respuesta);
                lblSaldo.ForeColor = UiTheme.Exito;
            }
            else
            {
                lblEstado.Text = mensaje;
                lblSaldo.Text = "No disponible";
                lblSaldo.ForeColor = UiTheme.Advertencia;
            }

            btnConsultar.Enabled = true;
        }
    }
}
