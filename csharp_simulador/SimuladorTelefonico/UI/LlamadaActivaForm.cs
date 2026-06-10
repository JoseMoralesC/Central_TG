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
        private readonly int _tiempoMaximoSegundos;
        private readonly string _tipoLlamadaDestino;
        private readonly decimal _costoPorMinuto;

        private Label lblDuracion = null!;
        private Label lblEstado = null!;
        private Label lblCosto = null!;
        private Button btnFinalizar = null!;
        private System.Windows.Forms.Timer timer = null!;
        private bool _finalizacionEnCurso;

        private readonly TramaService _tramaService = new();
        private readonly RespuestaService _respuestaService = new();
        private readonly CryptoService _cryptoService = new();

        public LlamadaActivaForm(
            string numeroDestino,
            string idLlamada,
            int tiempoMaximoSegundos,
            string tipoLlamadaDestino,
            decimal costoPorMinuto)
        {
            _numeroDestino = numeroDestino;
            _inicioLlamada = DateTime.Now;
            _idLlamada = idLlamada;
            _tiempoMaximoSegundos = tiempoMaximoSegundos;
            _tipoLlamadaDestino = string.IsNullOrWhiteSpace(tipoLlamadaDestino)
                ? AppConfig.TipoLlamada
                : tipoLlamadaDestino;
            _costoPorMinuto = costoPorMinuto > 0
                ? costoPorMinuto
                : AppConfig.CostoPorMinuto;

            ConfigurarVentana();
            ConstruirFormulario();
            IniciarTemporizador();

            Shown += async (s, e) => await EnviarInicioLlamadaAsync();
            FormClosed += (s, e) => LiberarTemporizador();
        }

        private void ConfigurarVentana()
        {
            Text = "Llamada activa";
            UiTheme.ConfigurarVentana(this, new Size(460, 540));
        }

        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Llamada activa",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 76
            };

            Label lblOrigen = UiTheme.CrearEtiqueta(
                $"Origen: {AppConfig.NumeroOrigen}",
                48,
                88,
                364,
                24,
                9.5f,
                FontStyle.Bold,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            Label lblNumeroDestino = UiTheme.CrearEtiqueta(
                $"Destino: {_numeroDestino}",
                48,
                122,
                364,
                34,
                13,
                FontStyle.Bold,
                UiTheme.Texto,
                ContentAlignment.MiddleCenter);

            lblDuracion = UiTheme.CrearEtiqueta(
                "00:00:00",
                48,
                184,
                364,
                78,
                34,
                FontStyle.Bold,
                Color.FromArgb(68, 199, 255),
                ContentAlignment.MiddleCenter);

            lblCosto = UiTheme.CrearEtiqueta(
                $"Costo estimado: {_costoPorMinuto:N2} {AppConfig.Moneda} por minuto",
                48,
                270,
                364,
                26,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            btnFinalizar = UiTheme.CrearBoton(
                "Finalizar llamada",
                86,
                326,
                288,
                44,
                UiTheme.Error);

            lblEstado = UiTheme.CrearEtiqueta(
                $"Preparando inicio. Limite: {_tiempoMaximoSegundos}s.",
                48,
                404,
                364,
                56,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            btnFinalizar.Click += async (s, e) => await FinalizarLlamadaAsync();

            Controls.Add(titulo);
            Controls.Add(lblOrigen);
            Controls.Add(lblNumeroDestino);
            Controls.Add(lblDuracion);
            Controls.Add(lblCosto);
            Controls.Add(btnFinalizar);
            Controls.Add(lblEstado);
        }

        private void IniciarTemporizador()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };

            timer.Tick += async (s, e) =>
            {
                TimeSpan duracion = DateTime.Now - _inicioLlamada;
                lblDuracion.Text = duracion.ToString(@"hh\:mm\:ss");

                if (_tiempoMaximoSegundos > 0
                    && duracion.TotalSeconds >= _tiempoMaximoSegundos
                    && !_finalizacionEnCurso)
                {
                    await FinalizarLlamadaAsync("SALDO_AGOTADO");
                }
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
                    TelefonoOrigen =
                        _cryptoService.CifrarDatoSensible(AppConfig.NumeroOrigen),
                    IdentificadorTelefono =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTelefono),
                    IdentificadorDispositivo =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorDispositivo),
                    IdentificadorTarjeta =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTarjeta),
                    TelefonoDestino = _numeroDestino,
                    Ubicacion = new UbicacionTelefono
                    {
                        Pais = AppConfig.Pais,
                        Provincia = AppConfig.Provincia,
                        Latitud = AppConfig.Latitud,
                        Longitud = AppConfig.Longitud
                    },
                    FechaInicio = _inicioLlamada.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Estado = "ACTIVA"
                },

                Control = new ControlInicioLlamada
                {
                    TipoServicio = AppConfig.TipoServicio,
                    TiempoMaximoSegundos = _tiempoMaximoSegundos,
                    ValidarSaldo = true,
                    MonitoreoActivo = true
                }
            };

            lblEstado.Text = "Registrando inicio de llamada...";

            string respuesta = await _tramaService.EnviarTramaAsync(inicio);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            if (!_respuestaService.EsRespuestaExitosa(respuesta))
            {
                timer.Stop();
                btnFinalizar.Enabled = false;
                lblEstado.Text = $"No fue posible iniciar. {mensaje}";
                lblEstado.ForeColor = UiTheme.Error;

                MessageBox.Show(
                    mensaje,
                    "Error al iniciar llamada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            lblEstado.Text = mensaje;
            lblEstado.ForeColor = UiTheme.Exito;
        }

        private async Task FinalizarLlamadaAsync(string motivoFinalizacion = "FINALIZACION_MANUAL")
        {
            if (_finalizacionEnCurso)
            {
                return;
            }

            _finalizacionEnCurso = true;
            btnFinalizar.Enabled = false;
            timer.Stop();

            DateTime fechaFin = DateTime.Now;
            TimeSpan duracion = fechaFin - _inicioLlamada;
            bool cierrePorSaldoAgotado =
                motivoFinalizacion.Equals("SALDO_AGOTADO", StringComparison.OrdinalIgnoreCase);

            int duracionSegundos = CalcularDuracionSegundos(duracion, cierrePorSaldoAgotado);
            int duracionMinutos = Math.Max(
                1,
                (int)Math.Ceiling(duracionSegundos / 60.0));

            decimal montoTotal = CalcularMontoTotal(duracionMinutos, cierrePorSaldoAgotado);

            FinalizarLlamada finalizar = new FinalizarLlamada
            {
                DatosLlamada = new DatosFinalizarLlamada
                {
                    IdLlamada = _idLlamada,
                    TelefonoOrigen =
                        _cryptoService.CifrarDatoSensible(AppConfig.NumeroOrigen),
                    IdentificadorTelefono =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTelefono),
                    IdentificadorDispositivo =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorDispositivo),
                    IdentificadorTarjeta =
                        _cryptoService.CifrarDatoSensible(AppConfig.IdentificadorTarjeta),
                    TelefonoDestino = _numeroDestino,
                    Ubicacion = new UbicacionTelefono
                    {
                        Pais = AppConfig.Pais,
                        Provincia = AppConfig.Provincia,
                        Latitud = AppConfig.Latitud,
                        Longitud = AppConfig.Longitud
                    },
                    FechaInicio = _inicioLlamada.ToString("yyyy-MM-ddTHH:mm:ss"),
                    FechaFin = fechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DuracionSegundos = duracionSegundos,
                    DuracionMinutos = duracionMinutos,
                    MotivoFinalizacion = motivoFinalizacion
                },

                DatosCobro = new DatosCobroLlamada
                {
                    TipoServicio = AppConfig.TipoServicio,
                    TipoLlamada = _tipoLlamadaDestino,
                    CostoPorMinuto = _costoPorMinuto,
                    MontoTotal = montoTotal,
                    Moneda = AppConfig.Moneda
                }
            };

            lblEstado.Text = "Registrando finalizacion y cobro...";

            string respuesta = await _tramaService.EnviarTramaAsync(finalizar);
            string mensaje = _respuestaService.ObtenerMensaje(respuesta);

            if (!_respuestaService.EsRespuestaExitosa(respuesta))
            {
                lblEstado.Text = mensaje;
                lblEstado.ForeColor = UiTheme.Error;

                MessageBox.Show(
                    "La llamada se detuvo localmente, pero el registro remoto respondio con error.\n\n" + mensaje,
                    "Error al finalizar llamada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
                return;
            }

            if (_respuestaService.TryObtenerSaldoDecimal(respuesta, out decimal saldoActualizado))
            {
                AppConfig.ActualizarSaldoTelefonoActual(saldoActualizado);
            }

            string motivoMensaje = cierrePorSaldoAgotado
                ? "La llamada finalizo porque se agoto el saldo disponible."
                : "Llamada finalizada correctamente.";

            MessageBox.Show(
                $"{motivoMensaje}\nDuracion cobrada: {duracionMinutos} minuto(s).\nMonto: {montoTotal:N2} {AppConfig.Moneda}.\nSaldo actual: {UiTheme.FormatearSaldo(AppConfig.TipoServicio, AppConfig.TelefonoActual.SaldoDisponible)}",
                "Llamada finalizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Close();
        }

        private decimal CalcularMontoTotal(int duracionMinutos, bool cierrePorSaldoAgotado)
        {
            decimal montoCalculado = duracionMinutos * _costoPorMinuto;

            if (!cierrePorSaldoAgotado
                || !AppConfig.TipoServicio.Equals("PREPAGO", StringComparison.OrdinalIgnoreCase))
            {
                return montoCalculado;
            }

            decimal saldoDisponible = Math.Max(0, AppConfig.TelefonoActual.SaldoDisponible);
            return Math.Min(montoCalculado, saldoDisponible);
        }

        private int CalcularDuracionSegundos(TimeSpan duracion, bool cierrePorSaldoAgotado)
        {
            int segundosReales = Math.Max(0, (int)Math.Ceiling(duracion.TotalSeconds));

            if (!cierrePorSaldoAgotado || _tiempoMaximoSegundos <= 0)
            {
                return segundosReales;
            }

            return Math.Min(segundosReales, _tiempoMaximoSegundos);
        }

        private void LiberarTemporizador()
        {
            if (timer == null)
            {
                return;
            }

            timer.Stop();
            timer.Dispose();
        }
    }
}
