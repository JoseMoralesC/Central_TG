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

        public MarcarNumeroForm()
        {
            ConfigurarVentana();
            ConstruirFormulario();
        }


        private void ConfigurarVentana()
        {
            Text = "Marcar número";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(420, 520);
            MinimumSize = new Size(420, 520);
            MaximizeBox = false;
        }


        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Marcar número",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };


            Label instruccion = new Label
            {
                Text = "Ingrese número destino (8 dígitos):",
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 100,
                Left = 30,
                Width = 340,
                Height = 30
            };


            txtNumeroDestino = new TextBox
            {
                Font = new Font("Segoe UI", 16),
                TextAlign = HorizontalAlignment.Center,
                Top = 145,
                Left = 60,
                Width = 280,
                MaxLength = 8
            };


            btnSolicitarLlamada = CrearBoton(
                "Solicitar llamada",
                220
            );


            btnVolver = CrearBoton(
                "Volver",
                290
            );


            lblEstado = new Label
            {
                Text = "Estado: esperando número.",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 360,
                Left = 30,
                Width = 340,
                Height = 60
            };


            btnSolicitarLlamada.Click += async (s, e) =>
                await SolicitarLlamadaAsync();


            btnVolver.Click += (s, e) =>
                Close();


            Controls.Add(titulo);
            Controls.Add(instruccion);
            Controls.Add(txtNumeroDestino);
            Controls.Add(btnSolicitarLlamada);
            Controls.Add(btnVolver);
            Controls.Add(lblEstado);
        }


        private Button CrearBoton(string texto, int top)
        {
            return new Button
            {
                Text = texto,
                Font = new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold
                ),
                Width = 260,
                Height = 48,
                Left = 75,
                Top = top
            };
        }


        private async Task SolicitarLlamadaAsync()
        {
            string numeroDestino =
                txtNumeroDestino.Text.Trim();


            if (!ValidacionTelefono.EsNumeroValido(numeroDestino))
            {
                MessageBox.Show(
                    "El número destino debe contener exactamente 8 dígitos.",
                    "Número inválido"
                );

                return;
            }


            btnSolicitarLlamada.Enabled = false;


            lblEstado.Text =
                "Estado: enviando solicitud al Identificador Python...";


            SolicitudLlamada solicitud = new SolicitudLlamada
            {
                TelefonoOrigen =
                    AppConfig.NumeroOrigen,


                TelefonoDestino =
                    numeroDestino,


                IdentificadorDispositivo =
                    AppConfig.IdentificadorDispositivo,


                IdentificadorTarjeta =
                    AppConfig.IdentificadorTarjeta,


                TipoLlamada =
                    AppConfig.TipoLlamada,


                FechaHora =
                    DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),


                Ubicacion = new UbicacionTelefono
                {
                    Pais =
                        AppConfig.Pais,

                    Provincia =
                        AppConfig.Provincia,

                    Latitud =
                        AppConfig.Latitud,

                    Longitud =
                        AppConfig.Longitud
                }
            };


            string json =
                _tramaService.ConvertirAJson(solicitud);


            string respuesta =
                await _tramaService.EnviarTramaAsync(solicitud);


            string mensaje =
                _respuestaService.ObtenerMensaje(respuesta);


            lblEstado.Text =
                $"Estado: respuesta recibida.\n{mensaje}";


            MessageBox.Show(
                $"Trama enviada:\n\n{json}\n\nRespuesta:\n\n{respuesta}",
                "Respuesta Identificador"
            );


            if (_respuestaService.EsRespuestaExitosa(respuesta))
            {
                using LlamadaActivaForm llamada =
                    new LlamadaActivaForm(numeroDestino);


                llamada.ShowDialog(this);
            }


            btnSolicitarLlamada.Enabled = true;
        }
    }
}