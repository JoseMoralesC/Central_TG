using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.UI
{
    public class SeleccionTelefonoForm : Form
    {
        private readonly List<CheckBox> _opcionesTelefono = new();
        private Label _lblEstado = null!;

        public SeleccionTelefonoForm()
        {
            Text = "Seleccionar telefonos virtuales";
            Size = new Size(760, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(16, 18, 22);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Label lblTitulo = new Label
            {
                Text = "Simulador Telefonico",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 22),
                Size = new Size(680, 40)
            };

            Label lblSubtitulo = new Label
            {
                Text = "Seleccione manualmente uno o varios telefonos activos para usar.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 68),
                Size = new Size(680, 25)
            };

            Label lblFuente = new Label
            {
                Text = AppConfig.FuenteDatosTelefonos,
                ForeColor = Color.FromArgb(255, 192, 88),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 94),
                Size = new Size(680, 22)
            };

            FlowLayoutPanel listaTelefonos = new FlowLayoutPanel
            {
                Location = new Point(40, 130),
                Size = new Size(680, 375),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(22, 25, 30),
                Padding = new Padding(10)
            };

            foreach (TelefonoVirtual telefono in AppConfig.TelefonosVirtuales)
            {
                CheckBox opcion = CrearOpcionTelefono(telefono);
                _opcionesTelefono.Add(opcion);
                listaTelefonos.Controls.Add(opcion);
            }

            _lblEstado = new Label
            {
                Text = "Listo para seleccionar.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 515),
                Size = new Size(680, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnAbrir = new Button
            {
                Text = "Abrir telefonos seleccionados",
                Location = new Point(200, 550),
                Size = new Size(360, 44),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.Click += AbrirTelefonosSeleccionados_Click;

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(lblFuente);
            Controls.Add(listaTelefonos);
            Controls.Add(_lblEstado);
            Controls.Add(btnAbrir);
        }

        private CheckBox CrearOpcionTelefono(TelefonoVirtual telefono)
        {
            CheckBox opcion = new CheckBox
            {
                Text =
                    $"{telefono.Numero}  |  {telefono.Cliente}\n" +
                    $"{telefono.TipoServicio}  |  Saldo: {telefono.SaldoDisponible:N2} CRC  |  {telefono.Proveedor}\n" +
                    $"SIM: {telefono.IdentificadorTarjeta}  |  IMEI: {telefono.IdentificadorDispositivo}",
                Tag = telefono,
                Size = new Size(640, 82),
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(12, 8, 8, 8),
                BackColor = telefono.Activo
                    ? Color.FromArgb(30, 34, 40)
                    : Color.FromArgb(42, 34, 34),
                ForeColor = telefono.Activo ? Color.White : Color.FromArgb(190, 170, 170),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = telefono.Activo,
                AutoSize = false,
                Cursor = Cursors.Hand
            };

            if (!telefono.Activo)
            {
                opcion.Text += "\nNo disponible: servicio inactivo en database.";
            }

            return opcion;
        }

        private void AbrirTelefonosSeleccionados_Click(object? sender, EventArgs e)
        {
            List<TelefonoVirtual> seleccionados = _opcionesTelefono
                .Where(opcion => opcion.Checked && opcion.Tag is TelefonoVirtual)
                .Select(opcion => opcion.Tag)
                .OfType<TelefonoVirtual>()
                .ToList();

            if (seleccionados.Count == 0)
            {
                _lblEstado.Text = "Seleccione al menos un telefono activo.";
                return;
            }

            AppConfig.SeleccionarTelefono(seleccionados[0].Id);

            Form1 pantallaPrincipal = new Form1(seleccionados);
            pantallaPrincipal.FormClosed += (s, args) => Show();
            pantallaPrincipal.Show();

            Hide();
        }
    }
}
