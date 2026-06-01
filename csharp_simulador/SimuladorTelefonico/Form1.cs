using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimuladorTelefonico.UI;

namespace SimuladorTelefonico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigurarVentana();
            ConstruirPantallaPrincipal();
        }

        private void ConfigurarVentana()
        {
            Text = "Simulador Telefónico";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(420, 620);
            MinimumSize = new Size(420, 620);
            MaximizeBox = false;
        }

        private void ConstruirPantallaPrincipal()
        {
            Controls.Clear();

            Label titulo = new Label
            {
                Text = "Simulador Telefónico",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };

            Label subtitulo = new Label
            {
                Text = "Central Telefónica - Módulo C#",
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            Button btnMarcar = CrearBoton("Marcar número", 130);
            Button btnConsultarSaldo = CrearBoton("Consultar saldo #9090*", 200);
            Button btnVerBitacora = CrearBoton("Ver bitácora local", 270);
            Button btnSalir = CrearBoton("Salir", 340);

            btnMarcar.Click += (s, e) =>
            {
                using MarcarNumeroForm form = new MarcarNumeroForm();
                form.ShowDialog(this);
            };

            btnConsultarSaldo.Click += (s, e) =>
            {
                using ConsultaSaldoForm form = new ConsultaSaldoForm();
                form.ShowDialog(this);
            };

            btnVerBitacora.Click += (s, e) =>
            {
                AbrirBitacoraLocal();
            };

            btnSalir.Click += (s, e) =>
            {
                Close();
            };

            Controls.Add(btnSalir);
            Controls.Add(btnVerBitacora);
            Controls.Add(btnConsultarSaldo);
            Controls.Add(btnMarcar);
            Controls.Add(subtitulo);
            Controls.Add(titulo);
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

        private void AbrirBitacoraLocal()
        {
            string rutaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            string rutaBitacora = Path.Combine(rutaLogs, "bitacora_simulador.txt");

            if (!Directory.Exists(rutaLogs))
            {
                Directory.CreateDirectory(rutaLogs);
            }

            if (!File.Exists(rutaBitacora))
            {
                File.WriteAllText(rutaBitacora, "Bitácora local del Simulador Telefónico C#\n");
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = rutaBitacora,
                UseShellExecute = true
            });
        }
    }
}