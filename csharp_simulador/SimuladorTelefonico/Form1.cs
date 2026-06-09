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

        private const int AnchoTarjetaTelefono = 390;
        private const int AltoTarjetaTelefono = 720;
        private const int SeparacionTarjeta = 18;

        private FlowLayoutPanel? _contenedorTelefonos;

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
            Size = new Size(1280, 820);
            MinimumSize = new Size(1040, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UiTheme.Fondo;

            DoubleBuffered = true;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true
            );
            UpdateStyles();

            Controls.Clear();
            CrearInterfaz();
        }

        private void RefrescarInterfaz()
        {
            if (_contenedorTelefonos == null)
            {
                return;
            }

            SuspendLayout();
            _contenedorTelefonos.SuspendLayout();

            _contenedorTelefonos.Controls.Clear();

            foreach (TelefonoVirtual telefono in _telefonosSeleccionados)
            {
                _contenedorTelefonos.Controls.Add(CrearCelular(telefono));
            }

            CentrarTarjetas(_contenedorTelefonos);

            _contenedorTelefonos.ResumeLayout(true);
            ResumeLayout(true);
        }

        private void CrearInterfaz()
        {
            SuspendLayout();

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 135,
                BackColor = UiTheme.Superficie
            };

            Button btnVolver = UiTheme.CrearBoton(
                "<  Volver",
                24,
                36,
                150,
                42,
                UiTheme.Primario);
            btnVolver.Click += (sender, e) => Close();

            Label lblTitulo = new Label
            {
                Text = "Telefonos en uso",
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 23, FontStyle.Bold),
                Location = new Point(0, 14),
                Size = new Size(ClientSize.Width, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitulo = new Label
            {
                Text =
                    $"Servidor: {AppConfig.HostIdentificador}:{AppConfig.PuertoIdentificador}  |  " +
                    $"Seleccionados: {_telefonosSeleccionados.Count}",
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 10),
                Location = new Point(0, 54),
                Size = new Size(ClientSize.Width, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblFuente = new Label
            {
                Text = AppConfig.FuenteDatosTelefonos,
                ForeColor = UiTheme.Exito,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 78),
                Size = new Size(ClientSize.Width, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };

            encabezado.Controls.Add(btnVolver);
            encabezado.Controls.Add(lblTitulo);
            encabezado.Controls.Add(lblSubtitulo);
            encabezado.Controls.Add(lblFuente);

            _contenedorTelefonos = CrearContenedorTelefonos();

            foreach (TelefonoVirtual telefono in _telefonosSeleccionados)
            {
                _contenedorTelefonos.Controls.Add(CrearCelular(telefono));
            }

            Controls.Add(_contenedorTelefonos);
            Controls.Add(encabezado);

            encabezado.BringToFront();

            CentrarTarjetas(_contenedorTelefonos);

            ResumeLayout(true);
        }

        private FlowLayoutPanel CrearContenedorTelefonos()
        {
            FlowLayoutPanel contenedor = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = UiTheme.Fondo,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(24, 36, 24, 36)
            };

            contenedor.Resize += (sender, e) => CentrarTarjetas(contenedor);

            return contenedor;
        }

        private void CentrarTarjetas(FlowLayoutPanel contenedor)
        {
            int anchoDisponible = contenedor.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            int anchoConMargen = AnchoTarjetaTelefono + (SeparacionTarjeta * 2);

            int tarjetasPorFila = Math.Max(1, anchoDisponible / anchoConMargen);
            int tarjetasEnFila = Math.Min(tarjetasPorFila, _telefonosSeleccionados.Count);

            int anchoFila = tarjetasEnFila * anchoConMargen;
            int margenIzquierdo = Math.Max(24, (anchoDisponible - anchoFila) / 2);

            int filas = (int)Math.Ceiling(_telefonosSeleccionados.Count / (double)tarjetasPorFila);
            int altoFila = AltoTarjetaTelefono + (SeparacionTarjeta * 2);
            int altoTotal = filas * altoFila;

            int margenSuperior = Math.Max(28, (contenedor.ClientSize.Height - altoTotal) / 2);

            contenedor.Padding = new Padding(margenIzquierdo, margenSuperior, 24, 36);
        }

        private Panel CrearCelular(TelefonoVirtual telefono)
        {
            RoundedPanel tarjeta = UiTheme.CrearPanelRedondeado(
                0,
                0,
                AnchoTarjetaTelefono,
                AltoTarjetaTelefono,
                Color.FromArgb(7, 10, 14),
                32);

            tarjeta.Margin = new Padding(SeparacionTarjeta);
            tarjeta.MinimumSize = new Size(AnchoTarjetaTelefono, AltoTarjetaTelefono);
            tarjeta.MaximumSize = new Size(AnchoTarjetaTelefono, AltoTarjetaTelefono);
            tarjeta.BorderColor = Color.FromArgb(55, 64, 78);

            Panel speaker = new Panel
            {
                Location = new Point(158, 18),
                Size = new Size(74, 6),
                BackColor = Color.FromArgb(55, 64, 78)
            };

            RoundedPanel pantalla = UiTheme.CrearPanelRedondeado(
                18,
                38,
                354,
                650,
                UiTheme.SuperficieElevada,
                24);

            pantalla.BorderColor = Color.FromArgb(35, 43, 54);

            Panel franja = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = telefono.TipoServicio == "PREPAGO"
                    ? UiTheme.Primario
                    : UiTheme.Postpago
            };

            Label lblServicio = new Label
            {
                Text = telefono.TipoServicio,
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(18, 18),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblEstado = new Label
            {
                Text = "Activo",
                ForeColor = UiTheme.Exito,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(166, 18),
                Size = new Size(110, 20),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label lblNombre = new Label
            {
                Text = FormatearNombreCliente(telefono.Cliente),
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(18, 48),
                Size = new Size(318, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true
            };

            Label lblNumero = new Label
            {
                Text = telefono.Numero,
                ForeColor = Color.FromArgb(68, 199, 255),
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Location = new Point(18, 78),
                Size = new Size(318, 48),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true
            };

            Label lblResumen = new Label
            {
                Text = $"{telefono.Proveedor}{Environment.NewLine}{telefono.Pais} | {telefono.Nacionalidad}",
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(18, 128),
                Size = new Size(318, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = false
            };

            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(lblResumen, $"{telefono.Proveedor} | {telefono.Pais} | {telefono.Nacionalidad}");

            RoundedPanel datos = UiTheme.CrearPanelRedondeado(
                18,
                184,
                318,
                174,
                UiTheme.PanelDatos,
                16);

            datos.BorderColor = Color.FromArgb(25, 32, 42);

            datos.Controls.Add(CrearDato("Saldo", UiTheme.FormatearSaldo(telefono.TipoServicio, telefono.SaldoDisponible), 10, 318));
            datos.Controls.Add(CrearDato("SIM", UiTheme.AgruparIdentificador(telefono.IdentificadorTarjeta), 62, 318));
            datos.Controls.Add(CrearDato("IMEI", UiTheme.AgruparIdentificador(telefono.IdentificadorDispositivo), 114, 318));

            tooltip.SetToolTip(datos, $"SIM: {telefono.IdentificadorTarjeta}\nIMEI: {telefono.IdentificadorDispositivo}");

            Label lblAcciones = new Label
            {
                Text = "Acciones",
                Location = new Point(18, 382),
                Size = new Size(318, 20),
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnMarcar = CrearBoton("Marcar", 410, UiTheme.Primario, true);
            btnMarcar.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                CambiarEstadoTelefono(lblEstado, true);

                using MarcarNumeroForm form = new MarcarNumeroForm();
                form.ShowDialog(this);

                CambiarEstadoTelefono(lblEstado, false);
                RefrescarInterfaz();
            };

            Button btnSaldo = CrearBoton("$  Saldo", 458, UiTheme.Primario, false);
            btnSaldo.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);

                using ConsultaSaldoForm form = new ConsultaSaldoForm();
                form.ShowDialog(this);

                RefrescarInterfaz();
            };

            Button btnBitacora = CrearBoton("Bitacora", 506, UiTheme.Primario, false);
            btnBitacora.Click += (sender, e) =>
            {
                AppConfig.SeleccionarTelefono(telefono.Id);
                MostrarBitacora();
            };

            pantalla.Controls.Add(franja);
            pantalla.Controls.Add(lblServicio);
            pantalla.Controls.Add(lblEstado);
            pantalla.Controls.Add(lblNombre);
            pantalla.Controls.Add(lblNumero);
            pantalla.Controls.Add(lblResumen);
            pantalla.Controls.Add(datos);
            pantalla.Controls.Add(lblAcciones);
            pantalla.Controls.Add(btnMarcar);
            pantalla.Controls.Add(btnSaldo);
            pantalla.Controls.Add(btnBitacora);

            tarjeta.Controls.Add(speaker);
            tarjeta.Controls.Add(pantalla);

            return tarjeta;
        }

        private Control CrearDato(string etiqueta, string valor, int y, int ancho)
        {
            Panel contenedor = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(ancho, 48),
                BackColor = Color.Transparent
            };

            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Location = new Point(14, 0),
                Size = new Size(ancho - 28, 18),
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblValor = new Label
            {
                Text = valor,
                Location = new Point(14, 18),
                Size = new Size(ancho - 28, 28),
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 9),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            contenedor.Controls.Add(lblEtiqueta);
            contenedor.Controls.Add(lblValor);

            return contenedor;
        }

        private Button CrearBoton(string texto, int y, Color color, bool principal)
        {
            return UiTheme.CrearBoton(texto, 24, y, 306, 36, color);
        }

        private static string FormatearNombreCliente(string nombre)
        {
            const string prefijo = "Cliente ";

            if (nombre.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
            {
                return nombre[prefijo.Length..];
            }

            return nombre;
        }

        private void CambiarEstadoTelefono(Label lblEstado, bool enLlamada)
        {
            lblEstado.Text = enLlamada ? "En llamada" : "Activo";
            lblEstado.ForeColor = enLlamada
                ? UiTheme.Advertencia
                : UiTheme.Exito;
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

            using BitacoraForm bitacora = new BitacoraForm(contenido);
            bitacora.ShowDialog(this);
        }
    }
}
