using System;
using System.Collections.Generic;
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
        private readonly List<TelefonoVirtual> _telefonosSeleccionados;
        private const int AnchoTarjetaTelefono = 252;
        private const int AltoTarjetaTelefono = 500;
        private const int SeparacionTarjeta = 10;

        public Form1()
            : this(AppConfig.TelefonosVirtuales.Where(t => t.Activo).Take(1))
        {
        }

        public Form1(IEnumerable<TelefonoVirtual> telefonosSeleccionados)
        {
            _telefonosSeleccionados = telefonosSeleccionados
                .Where(t => t.Activo)
                .ToList();

            if (_telefonosSeleccionados.Count == 0)
            {
                _telefonosSeleccionados.Add(AppConfig.TelefonosVirtuales.First(t => t.Activo));
            }

            InitializeComponent();

            Text = "Simulador Telefonico - Central TG";
            Size = new Size(1180, 720);
            MinimumSize = new Size(920, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(12, 14, 18);

            Controls.Clear();

            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 112,
                BackColor = Color.FromArgb(18, 21, 26)
            };

            Button btnVolver = new Button
            {
                Text = "Volver al selector",
                Location = new Point(24, 36),
                Size = new Size(190, 40),
                BackColor = Color.FromArgb(52, 58, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (sender, e) => Close();

            Label lblTitulo = new Label
            {
                Text = "Telefonos en uso",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(238, 14),
                Size = new Size(600, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSubtitulo = new Label
            {
                Text =
                    $"Identificador: {AppConfig.HostIdentificador}:{AppConfig.PuertoIdentificador}  |  " +
                    $"Seleccionados: {_telefonosSeleccionados.Count}",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                Location = new Point(240, 54),
                Size = new Size(680, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblFuente = new Label
            {
                Text = AppConfig.FuenteDatosTelefonos,
                ForeColor = Color.FromArgb(255, 192, 88),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(240, 80),
                Size = new Size(680, 22),
                TextAlign = ContentAlignment.MiddleLeft
            };

            encabezado.Controls.Add(btnVolver);
            encabezado.Controls.Add(lblTitulo);
            encabezado.Controls.Add(lblSubtitulo);
            encabezado.Controls.Add(lblFuente);
            FlowLayoutPanel contenedorTelefonos = CrearContenedorTelefonos();

            foreach (TelefonoVirtual telefono in _telefonosSeleccionados)
            {
                contenedorTelefonos.Controls.Add(CrearCelular(telefono));
            }

            Controls.Add(contenedorTelefonos);
            Controls.Add(encabezado);

            encabezado.BringToFront();

            CentrarTarjetas(contenedorTelefonos);
        }

        private FlowLayoutPanel CrearContenedorTelefonos()
        {
            FlowLayoutPanel contenedor = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(12, 14, 18),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(24, 55, 24, 18)
            };

            contenedor.Resize += (sender, e) => CentrarTarjetas(contenedor);

            return contenedor;
        }

        private void CentrarTarjetas(FlowLayoutPanel contenedor)
        {
            int anchoDisponible = contenedor.ClientSize.Width
                - SystemInformation.VerticalScrollBarWidth;
            int anchoConMargen = AnchoTarjetaTelefono + (SeparacionTarjeta * 2);
            int tarjetasPorFila = Math.Max(1, anchoDisponible / anchoConMargen);
            int tarjetasEnFila = Math.Min(tarjetasPorFila, _telefonosSeleccionados.Count);
            int anchoFila = tarjetasEnFila * anchoConMargen;
            int margenIzquierdo = Math.Max(24, (anchoDisponible - anchoFila) / 2);

            contenedor.Padding = new Padding(margenIzquierdo, 55, 24, 18);
        }

        private Panel CrearCelular(TelefonoVirtual telefono)
        {
            Panel tarjeta = new Panel
            {
                Size = new Size(AnchoTarjetaTelefono, AltoTarjetaTelefono),
                Margin = new Padding(SeparacionTarjeta),
                BackColor = Color.FromArgb(24, 28, 34),
                MinimumSize = new Size(AnchoTarjetaTelefono, AltoTarjetaTelefono)
            };

            Panel franja = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = telefono.TipoServicio == "PREPAGO"
                    ? Color.FromArgb(37, 167, 218)
                    : Color.FromArgb(129, 205, 124)
            };

            Label lblServicio = new Label
            {
                Text = telefono.TipoServicio,
                ForeColor = Color.FromArgb(218, 224, 232),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(18, 20),
                Size = new Size(92, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblEstado = new Label
            {
                Text = "Disponible",
                ForeColor = Color.FromArgb(76, 220, 130),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(126, 20),
                Size = new Size(112, 20),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label lblNombre = new Label
            {
                Text = FormatearNombreCliente(telefono.Cliente),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(18, 50),
                Size = new Size(224, 54),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = false
            };

            Label lblNumero = new Label
            {
                Text = telefono.Numero,
                ForeColor = Color.FromArgb(67, 198, 255),
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                Location = new Point(18, 112),
                Size = new Size(224, 42),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblResumen = new Label
            {
                Text = $"{telefono.Proveedor} | {telefono.TipoLlamada}",
                ForeColor = Color.FromArgb(172, 182, 194),
                Font = new Font("Segoe UI", 8),
                Location = new Point(18, 158),
                Size = new Size(224, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true
            };

            Panel datos = new Panel
            {
                Location = new Point(18, 190),
                Size = new Size(224, 108),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            datos.Controls.Add(CrearDato("Saldo", $"{telefono.SaldoDisponible:N2} CRC", 8));
            datos.Controls.Add(CrearDato("SIM", telefono.IdentificadorTarjeta, 39));
            datos.Controls.Add(CrearDato("IMEI", telefono.IdentificadorDispositivo, 70));

            Label lblAcciones = new Label
            {
                Text = "Acciones",
                Location = new Point(18, 316),
                Size = new Size(224, 20),
                ForeColor = Color.FromArgb(156, 166, 178),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnMarcar = CrearBoton("Marcar", 342, Color.FromArgb(0, 122, 204), true);
            btnMarcar.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                CambiarEstadoTelefono(lblEstado, true);
                using MarcarNumeroForm form = new MarcarNumeroForm();
                form.ShowDialog(this);
                CambiarEstadoTelefono(lblEstado, false);
            };

            Button btnSaldo = CrearBoton("Saldo", 390, Color.FromArgb(49, 58, 70), true);
            btnSaldo.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                using ConsultaSaldoForm form = new ConsultaSaldoForm();
                form.ShowDialog(this);
            };

            Label lblHistorial = new Label
            {
                Text = "Historial",
                Location = new Point(18, 438),
                Size = new Size(224, 18),
                ForeColor = Color.FromArgb(156, 166, 178),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnBitacora = CrearBoton("Bitacora", 460, Color.FromArgb(49, 58, 70), true);
            btnBitacora.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                MostrarBitacora();
            };

            tarjeta.Controls.Add(franja);
            tarjeta.Controls.Add(lblServicio);
            tarjeta.Controls.Add(lblEstado);
            tarjeta.Controls.Add(lblNombre);
            tarjeta.Controls.Add(lblNumero);
            tarjeta.Controls.Add(lblResumen);
            tarjeta.Controls.Add(datos);
            tarjeta.Controls.Add(lblAcciones);
            tarjeta.Controls.Add(btnMarcar);
            tarjeta.Controls.Add(btnSaldo);
            tarjeta.Controls.Add(lblHistorial);
            tarjeta.Controls.Add(btnBitacora);

            return tarjeta;
        }

        private Label CrearDato(string etiqueta, string valor, int y)
        {
            return new Label
            {
                Text = $"{etiqueta}: {valor}",
                Location = new Point(10, y),
                Size = new Size(204, 22),
                ForeColor = Color.FromArgb(218, 224, 232),
                Font = new Font("Segoe UI", 8),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Button CrearBoton(string texto, int y, Color color, bool principal)
        {
            Button boton = new Button
            {
                Text = texto,
                Location = new Point(18, y),
                Size = new Size(224, principal ? 36 : 34),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            boton.FlatAppearance.BorderSize = 0;

            return boton;
        }

        private static string FormatearNombreCliente(string nombre)
        {
            const string prefijo = "Cliente ";

            if (nombre.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
            {
                return $"Cliente:{Environment.NewLine}{nombre[prefijo.Length..]}";
            }

            return nombre;
        }

        private void CambiarEstadoTelefono(Label lblEstado, bool enLlamada)
        {
            lblEstado.Text = enLlamada ? "En llamada" : "Disponible";
            lblEstado.ForeColor = enLlamada
                ? Color.FromArgb(255, 192, 88)
                : Color.FromArgb(76, 220, 130);
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
                    "Todavia no existen registros en la bitacora local.",
                    "Bitacora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            string contenido = File.ReadAllText(rutaBitacora);

            MessageBox.Show(
                contenido,
                "Bitacora local del simulador",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
