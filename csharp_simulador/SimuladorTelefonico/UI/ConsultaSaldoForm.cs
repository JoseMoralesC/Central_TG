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
        private Button btnConsultar = null!;
        private Button btnVolver = null!;

        private readonly TramaService _tramaService = new();
        private readonly RespuestaService _respuestaService = new();

        public ConsultaSaldoForm()
        {
            ConfigurarVentana();
            ConstruirFormulario();
        }

        private void ConfigurarVentana()
        {
            Text = "Consultar saldo";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(420, 520);
            MinimumSize = new Size(420, 520);
            MaximizeBox = false;
        }

        private void ConstruirFormulario()
        {
            Label titulo = new Label
            {
                Text = "Consulta saldo",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };

            Label lblCodigo = new Label
            {
                Text = "Ingrese el código de consulta:",
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 120,
                Left = 30,
                Width = 340,
                Height = 30
            };

            txtCodigoConsulta = new TextBox
            {
                Font = new Font("Segoe UI", 18),
                TextAlign = HorizontalAlignment.Center,
                Top = 165,
                Left = 60,
                Width = 280,
                Text = "#9090*",
                MaxLength = 6
            };

            btnConsultar = CrearBoton("Consultar saldo", 245);
            btnVolver = CrearBoton("Volver", 310);

            lblEstado = new Label
            {
                Text = "Estado: esperando consulta.",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 385,
                Left = 30,
                Width = 340,
                Height = 60
            };

            btnConsultar.Click += async (s, e) =>
                await ConsultarSaldoAsync();

            btnVolver.Click += (s, e) => Close();

            Controls.Add(titulo);
            Controls.Add(lblCodigo);
            Controls.Add(txtCodigoConsulta);
            Controls.Add(btnConsultar);
            Controls.Add(btnVolver);
            Controls.Add(lblEstado);
        }

        private Button CrearBoton(string texto, int top)
        {
            return new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Width = 260,
                Height = 48,
                Left = 75,
                Top = top
            };
        }

        private async Task ConsultarSaldoAsync()
        {
            string codigoConsulta = txtCodigoConsulta.Text.Trim();

            if (!ValidacionTelefono.EsCodigoConsultaSaldo(codigoConsulta))
            {
                MessageBox.Show(
                    "Para consultar saldo debe ingresar exactamente el código #9090*.",
                    "Código inválido"
                );

                return;
            }

            btnConsultar.Enabled = false;

            lblEstado.Text =
                "Estado: enviando consulta al Identificador Python...";

            ConsultaSaldo consulta = new ConsultaSaldo
            {
                TelefonoOrigen = AppConfig.NumeroOrigen,

                IdentificadorDispositivo =
                    AppConfig.IdentificadorDispositivo,

                IdentificadorTarjeta =
                    AppConfig.IdentificadorTarjeta,

                FechaHora =
                    DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string json =
                _tramaService.ConvertirAJson(consulta);

            string respuesta =
                await _tramaService.EnviarTramaAsync(consulta);

            string mensaje =
                _respuestaService.ObtenerMensaje(respuesta);

            RespuestaSaldo? respuestaSaldo =
                _respuestaService.ConvertirRespuestaSaldo(respuesta);

            if (respuestaSaldo != null)
            {
                lblEstado.Text =
                    $"Estado: {respuestaSaldo.Resultado.Estado}\n" +
                    $"Saldo: {respuestaSaldo.Saldo.MontoDisponible} " +
                    $"{respuestaSaldo.Saldo.Moneda}";
            }
            else
            {
                lblEstado.Text =
                    $"Estado: respuesta recibida.\n{mensaje}";
            }

            MessageBox.Show(
                $"Consulta enviada:\n\n{json}\n\nRespuesta:\n\n{respuesta}",
                "Saldo"
            );

            btnConsultar.Enabled = true;
        }
    }
}