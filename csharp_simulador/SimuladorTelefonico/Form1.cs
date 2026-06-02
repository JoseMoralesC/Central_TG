using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
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
            Size = new Size(440, 660);
            MinimumSize = new Size(440, 660);
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

            Label configuracion = new Label
            {
                Text =
                    $"Origen: {AppConfig.NumeroOrigen}\n" +
                    $"Identificador: {AppConfig.HostIdentificador}:{AppConfig.PuertoIdentificador}\n" +
                    $"Servicio: {AppConfig.TipoServicio} / {AppConfig.TipoLlamada}",
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 115,
                Left = 30,
                Width = 360,
                Height = 70
            };

            Button btnMarcar = CrearBoton("Marcar número", 210);
            Button btnConsultarSaldo = CrearBoton("Consultar saldo #9090*", 280);
            Button btnVerBitacora = CrearBoton("Ver bitácora local", 350);
            Button btnSalir = CrearBoton("Salir", 420);

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
            Controls.Add(configuracion);
            Controls.Add(subtitulo);
            Controls.Add(titulo);
        }

        private Button CrearBoton(string texto, int top)
        {
            return new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Width = 280,
                Height = 50,
                Left = 70,
                Top = top
            };
        }

        private void AbrirBitacoraLocal()
        {
            string rutaLogs = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "logs"
            );

            string rutaBitacora = Path.Combine(
                rutaLogs,
                "bitacora_simulador.txt"
            );

            if (!Directory.Exists(rutaLogs))
            {
                Directory.CreateDirectory(rutaLogs);
            }

            if (!File.Exists(rutaBitacora))
            {
                File.WriteAllText(
                    rutaBitacora,
                    "Bitácora local del Simulador Telefónico C#\n"
                );
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = rutaBitacora,
                UseShellExecute = true
            });
        }
    }
}