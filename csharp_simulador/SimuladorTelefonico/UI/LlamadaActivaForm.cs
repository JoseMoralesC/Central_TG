using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;
using SimuladorTelefonico.Services;

namespace SimuladorTelefonico.UI
{
    public class LlamadaActivaForm : Form
    {
        private readonly string _numeroDestino;
        private readonly DateTime _inicioLlamada;
        private readonly string _idLlamada;

        private Label lblNumeroDestino = null!;
        private Label lblDuracion = null!;
        private Label lblEstado = null!;
        private Button btnFinalizar = null!;
        private System.Windows.Forms.Timer timer = null!;

        private readonly TramaService _tramaService = new();
        private readonly RespuestaService _respuestaService = new();

        public LlamadaActivaForm(string numeroDestino)
        {
            _numeroDestino = numeroDestino;
            _inicioLlamada = DateTime.Now;
            _idLlamada = $"CALL-{DateTime.Now:yyyyMMddHHmmss}";

            ConfigurarVentana();
            ConstruirFormulario();
            IniciarTemporizador();

            Shown += async (s, e) => await EnviarInicioLlamadaAsync();
        }

        private void ConfigurarVentana()
        {
            Text = "Llamada activa";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(420, 520);
            MinimumSize = new Size(420, 520);
            MaximizeBox = false;
        }

        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Llamada activa",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };

            lblNumeroDestino = new Label
            {
                Text = $"Destino: {_numeroDestino}",
                Font = new Font("Segoe UI", 13),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 120,
                Left = 30,
                Width = 340,
                Height = 40
            };

            lblDuracion = new Label
            {
                Text = "00:00:00",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 190,
                Left = 30,
                Width = 340,
                Height = 70
            };

            btnFinalizar = new Button
            {
                Text = "Finalizar llamada",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Width = 260,
                Height = 48,
                Left = 75,
                Top = 300
            };

            lblEstado = new Label
            {
                Text = "Estado: preparando inicio de llamada.",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 380,
                Left = 30,
                Width = 340,
                Height = 60
            };

            btnFinalizar.Click += async (s, e) => await FinalizarLlamadaAsync();

            Controls.Add(titulo);
            Controls.Add(lblNumeroDestino);
            Controls.Add(lblDuracion);
            Controls.Add(btnFinalizar);
            Controls.Add(lblEstado);
        }

        private void IniciarTemporizador()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };

            timer.Tick += (s, e) =>
            {
                TimeSpan duracion = DateTime.Now - _inicioLlamada;
                lblDuracion.Text = duracion.ToString(@"hh\:mm\:ss");
            };

            timer.Start();
        }

        private async Task EnviarInicioLlamadaAsync()
        {
            InicioLlamada inicio = new InicioLlamada
            {
                DatosLlamada = new DatosInicioLlamada
                {
                    IdLlamada = _idLlamada,
                    TelefonoOrigen = AppConfig.NumeroOrigen,
                    TelefonoDestino = _numeroDestino,
                    FechaInicio = _inicioLlamada.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Estado = "ACTIVA"
                },

                Control = new ControlInicioLlamada
                {
                    TipoServicio = AppConfig.TipoServicio,
                    TiempoMaximoSegundos = 1800,
                    ValidarSaldo = true,
                    MonitoreoActivo = true
                }
            };

            string json = _tramaService.ConvertirAJson(inicio);

            lblEstado.Text = "Estado: enviando inicio de llamada a Python...";

            string respuesta = await _tramaService.EnviarTramaAsync(inicio);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            if (!_respuestaService.EsRespuestaExitosa(respuesta))
            {
                timer.Stop();
                btnFinalizar.Enabled = false;

                lblEstado.Text = $"Estado: error al iniciar.\n{mensaje}";

                MessageBox.Show(
                    $"No fue posible iniciar la llamada.\n\nTrama:\n{json}\n\nRespuesta:\n{respuesta}",
                    "Error al iniciar llamada"
                );

                return;
            }

            lblEstado.Text = $"Estado: llamada activa.\n{mensaje}";

            MessageBox.Show(
                $"Inicio enviado:\n\n{json}\n\nRespuesta:\n\n{respuesta}",
                "Inicio de llamada"
            );
        }

        private async Task FinalizarLlamadaAsync()
        {
            btnFinalizar.Enabled = false;
            timer.Stop();

            DateTime fechaFin = DateTime.Now;
            TimeSpan duracion = fechaFin - _inicioLlamada;

            int duracionSegundos = (int)duracion.TotalSeconds;
            int duracionMinutos = Math.Max(
                1,
                (int)Math.Ceiling(duracion.TotalMinutes)
            );

            decimal montoTotal =
                duracionMinutos * AppConfig.CostoPorMinuto;

            FinalizarLlamada finalizar = new FinalizarLlamada
            {
                DatosLlamada = new DatosFinalizarLlamada
                {
                    IdLlamada = _idLlamada,
                    TelefonoOrigen = AppConfig.NumeroOrigen,
                    TelefonoDestino = _numeroDestino,
                    FechaInicio = _inicioLlamada.ToString("yyyy-MM-ddTHH:mm:ss"),
                    FechaFin = fechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DuracionSegundos = duracionSegundos,
                    DuracionMinutos = duracionMinutos,
                    MotivoFinalizacion = "FINALIZACION_MANUAL"
                },

                DatosCobro = new DatosCobroLlamada
                {
                    TipoServicio = AppConfig.TipoServicio,
                    TipoLlamada = AppConfig.TipoLlamada,
                    CostoPorMinuto = AppConfig.CostoPorMinuto,
                    MontoTotal = montoTotal,
                    Moneda = AppConfig.Moneda
                }
            };

            string json = _tramaService.ConvertirAJson(finalizar);

            lblEstado.Text = "Estado: enviando finalización a Python...";

            string respuesta = await _tramaService.EnviarTramaAsync(finalizar);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            if (!_respuestaService.EsRespuestaExitosa(respuesta))
            {
                lblEstado.Text = $"Estado: error al finalizar.\n{mensaje}";

                MessageBox.Show(
                    $"La llamada fue detenida localmente, pero Python respondió con error.\n\nTrama:\n{json}\n\nRespuesta:\n{respuesta}",
                    "Error al finalizar llamada"
                );

                Close();
                return;
            }

            lblEstado.Text = $"Estado: llamada finalizada.\n{mensaje}";

            MessageBox.Show(
                $"Finalización enviada:\n\n{json}\n\nRespuesta:\n\n{respuesta}",
                "Finalizar llamada"
            );

            Close();
        }
    }
}