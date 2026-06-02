using System;
using System.Drawing;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.UI
{
    public class SeleccionTelefonoForm : Form
    {
        public SeleccionTelefonoForm()
        {
            Text = "Seleccionar teléfono virtual";
            Size = new Size(520, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(18, 18, 18);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Label lblTitulo = new Label
            {
                Text = "Simulador Telefónico",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 25),
                Size = new Size(420, 40)
            };

            Label lblSubtitulo = new Label
            {
                Text = "Seleccione el teléfono virtual que desea utilizar",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 70),
                Size = new Size(420, 25)
            };

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);

            int y = 125;

            foreach (TelefonoVirtual telefono in AppConfig.TelefonosVirtuales)
            {
                Button btnTelefono = CrearBotonTelefono(telefono, y);
                Controls.Add(btnTelefono);
                y += 115;
            }
        }

        private Button CrearBotonTelefono(TelefonoVirtual telefono, int y)
        {
            Button boton = new Button
            {
                Text =
                    $"{telefono.Nombre}\n" +
                    $"{telefono.Numero}\n" +
                    $"{telefono.Maquina} - {telefono.TipoServicio}",
                Tag = telefono.Id,
                Location = new Point(75, y),
                Size = new Size(350, 90),
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            boton.FlatAppearance.BorderColor = Color.FromArgb(0, 180, 255);
            boton.FlatAppearance.BorderSize = 2;

            boton.Click += SeleccionarTelefono_Click;

            return boton;
        }

        private void SeleccionarTelefono_Click(object? sender, EventArgs e)
        {
            if (sender is not Button boton || boton.Tag is not string telefonoId)
            {
                return;
            }

            AppConfig.SeleccionarTelefono(telefonoId);

            Form1 pantallaPrincipal = new Form1();
            pantallaPrincipal.Show();

            Hide();
        }
    }
}