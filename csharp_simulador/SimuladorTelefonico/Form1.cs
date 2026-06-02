using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;
using SimuladorTelefonico.UI;

namespace SimuladorTelefonico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = "Simulador Telefónico - Central TG";
            Size = new Size(1000, 780);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(12, 12, 16);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Controls.Clear();

            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Label lblTitulo = new Label
            {
                Text = "Central TG - Simulador de Teléfonos Virtuales",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Location = new Point(40, 20),
                Size = new Size(900, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text = $"Identificador Python: {AppConfig.HostIdentificador}:{AppConfig.PuertoIdentificador}",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(40, 68),
                Size = new Size(900, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);

            TelefonoVirtual[] telefonos = AppConfig.TelefonosVirtuales
                .Where(t => t.Maquina == AppConfig.TelefonoActual.Maquina)
                .ToArray();

            int x = 115;

            foreach (TelefonoVirtual telefono in telefonos)
            {
                Panel celular = CrearCelular(telefono, x, 120);
                Controls.Add(celular);
                x += 390;
            }
        }

        private Panel CrearCelular(TelefonoVirtual telefono, int x, int y)
        {
            Panel carcasa = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(330, 600),
                BackColor = Color.FromArgb(32, 32, 36)
            };

            Panel pantalla = new Panel
            {
                Location = new Point(18, 18),
                Size = new Size(294, 540),
                BackColor = Color.FromArgb(5, 10, 18)
            };

            Label lblBarra = new Label
            {
                Text = "📶  Central TG                  🔋",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Location = new Point(12, 10),
                Size = new Size(270, 22)
            };

            Label lblHora = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(116, 35),
                Size = new Size(60, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblNombre = new Label
            {
                Text = telefono.Nombre,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 80),
                Size = new Size(250, 38),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblNumero = new Label
            {
                Text = telefono.Numero,
                ForeColor = Color.FromArgb(0, 200, 255),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(20, 122),
                Size = new Size(250, 42),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblDetalle = new Label
            {
                Text =
                    $"{telefono.Maquina}\n" +
                    $"{telefono.TipoServicio}\n" +
                    $"ID: {telefono.Id}",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 170),
                Size = new Size(250, 65),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnMarcar = CrearBoton("📞  Marcar", 260);
            btnMarcar.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                MarcarNumeroForm form = new MarcarNumeroForm();
                form.ShowDialog();
            };

            Button btnSaldo = CrearBoton("💰  Saldo", 320);
            btnSaldo.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                ConsultaSaldoForm form = new ConsultaSaldoForm();
                form.ShowDialog();
            };

            Button btnBitacora = CrearBoton("📄  Bitácora", 380);
            btnBitacora.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                MostrarBitacora();
            };

            Label lblActivo = new Label
            {
                Text = "Disponible",
                ForeColor = Color.FromArgb(0, 255, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 455),
                Size = new Size(250, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pantalla.Controls.Add(lblBarra);
            pantalla.Controls.Add(lblHora);
            pantalla.Controls.Add(lblNombre);
            pantalla.Controls.Add(lblNumero);
            pantalla.Controls.Add(lblDetalle);
            pantalla.Controls.Add(btnMarcar);
            pantalla.Controls.Add(btnSaldo);
            pantalla.Controls.Add(btnBitacora);
            pantalla.Controls.Add(lblActivo);

            carcasa.Controls.Add(pantalla);

            return carcasa;
        }

        private Button CrearBoton(string texto, int y)
        {
            Button boton = new Button
            {
                Text = texto,
                Location = new Point(47, y),
                Size = new Size(200, 44),
                BackColor = Color.FromArgb(0, 130, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            boton.FlatAppearance.BorderSize = 0;

            return boton;
        }

        private void MostrarBitacora()
        {
            string rutaBitacora = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "logs",
                "bitacora_simulador.txt"
            );

            if (!File.Exists(rutaBitacora))
            {
                MessageBox.Show(
                    "Todavía no existen registros en la bitácora local.",
                    "Bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            string contenido = File.ReadAllText(rutaBitacora);

            MessageBox.Show(
                contenido,
                "Bitácora local del simulador",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}