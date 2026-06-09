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
    public class MarcarNumeroForm : Form
    {
        private TextBox txtNumeroDestino = null!;
        private Button btnSolicitarLlamada = null!;
        private Button btnVolver = null!;
        private Label lblEstado = null!;

        private readonly TramaService _tramaService = new();
        private readonly RespuestaService _respuestaService = new();
        private readonly CryptoService _cryptoService = new();

        public MarcarNumeroForm()
        {
            ConfigurarVentana();
            ConstruirFormulario();
        }

        private void ConfigurarVentana()
        {
            Text = "Marcar numero";
            UiTheme.ConfigurarVentana(this, new Size(430, 500));
        }

        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Nueva llamada",
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 76
            };

            Label lblOrigen = UiTheme.CrearEtiqueta(
                $"Origen: {AppConfig.NumeroOrigen}",
                40,
                82,
                350,
                24,
                9.5f,
                FontStyle.Bold,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            Label instruccion = UiTheme.CrearEtiqueta(
                "Numero destino",
                40,
                128,
                350,
                24,
                10.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            txtNumeroDestino = new TextBox
            {
                Font = new Font("Segoe UI", 18),
                TextAlign = HorizontalAlignment.Center,
                Top = 158,
                Left = 70,
                Width = 290,
                Height = 40,
                MaxLength = 16
            };

            Label lblAyuda = UiTheme.CrearEtiqueta(
                "Use 8 digitos o #9090* para consultar saldo.",
                40,
                210,
                350,
                28,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            btnSolicitarLlamada = CrearBoton("Solicitar llamada", 270, UiTheme.Primario);
            btnVolver = CrearBoton("Volver", 322, Color.FromArgb(52, 60, 72));

            lblEstado = UiTheme.CrearEtiqueta(
                "Esperando numero destino.",
                40,
                388,
                350,
                48,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            btnSolicitarLlamada.Click += async (s, e) => await SolicitarLlamadaAsync();
            btnVolver.Click += (s, e) => Close();

            Controls.Add(titulo);
            Controls.Add(lblOrigen);
            Controls.Add(instruccion);
            Controls.Add(txtNumeroDestino);
            Controls.Add(lblAyuda);
            Controls.Add(btnSolicitarLlamada);
            Controls.Add(btnVolver);
            Controls.Add(lblEstado);
        }

        private Button CrearBoton(string texto, int top, Color color)
        {
            return UiTheme.CrearBoton(texto, 70, top, 290, 42, color);
        }

        private async Task SolicitarLlamadaAsync()
        {
            string numeroDestino = txtNumeroDestino.Text.Trim();

            if (ValidacionTelefono.EsCodigoConsultaSaldo(numeroDestino))
            {
                using ConsultaSaldoForm consultaSaldo = new ConsultaSaldoForm();
                consultaSaldo.ShowDialog(this);
                return;
            }

            if (!ValidacionTelefono.EsNumeroValido(numeroDestino))
            {
                MessageBox.Show(
                    "El numero destino debe contener exactamente 8 digitos.",
                    "Numero invalido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            btnSolicitarLlamada.Enabled = false;
            lblEstado.Text = "Validando llamada con el Identificador...";

            SolicitudLlamada solicitud = new SolicitudLlamada
            {
                TelefonoOrigen =
                    _cryptoService.CifrarDatoSensible(AppConfig.NumeroOrigen),

                TelefonoDestino = numeroDestino,

                IdentificadorTelefono =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTelefono),

                IdentificadorDispositivo =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorDispositivo),

                IdentificadorTarjeta =
                    _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTarjeta),

                TipoLlamada = AppConfig.TipoLlamada,

                FechaHora = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),

                Ubicacion = new UbicacionTelefono
                {
                    Pais = AppConfig.Pais,
                    Provincia = AppConfig.Provincia,
                    Latitud = AppConfig.Latitud,
                    Longitud = AppConfig.Longitud
                }
            };

            string respuesta = await _tramaService.EnviarTramaAsync(solicitud);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            lblEstado.Text = mensaje;

            if (_respuestaService.EsRespuestaExitosa(respuesta))
            {
                int tiempoMaximoSegundos =
                    _respuestaService.ObtenerTiempoMaximoSegundos(respuesta, 1800);

                string idLlamada =
                    _respuestaService.ObtenerIdLlamada(
                        respuesta,
                        $"CALL-{DateTime.Now:yyyyMMddHHmmss}");

                using LlamadaActivaForm llamada =
                    new LlamadaActivaForm(
                        numeroDestino,
                        idLlamada,
                        tiempoMaximoSegundos);

                llamada.ShowDialog(this);
                Close();
                return;
            }

            MessageBox.Show(
                mensaje,
                "Llamada no autorizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            btnSolicitarLlamada.Enabled = true;
        }
    }
}
