using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.UI
{
    public class SeleccionTelefonoForm : Form
    {
        private readonly List<CheckBox> _opcionesTelefono = new();
        private readonly System.Windows.Forms.Timer _pollingTimer = new();
        private Label _lblEstado = null!;
        private Label _lblFuente = null!;
        private FlowLayoutPanel _listaTelefonos = null!;
        private bool _actualizandoCatalogo;

        public SeleccionTelefonoForm()
        {
            Text = "Seleccionar telefonos virtuales";
            UiTheme.ConfigurarVentana(this, new Size(920, 720), new Size(860, 680));
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            DoubleBuffered = true;

            CrearInterfaz();
            Shown += SeleccionTelefonoForm_Shown;
            FormClosed += (_, _) => _pollingTimer.Dispose();
        }

        private void CrearInterfaz()
        {
            Label lblTitulo = new Label
            {
                Text = "Simulador Telefonico",
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 24),
                Size = new Size(780, 42)
            };

            Label lblSubtitulo = UiTheme.CrearEtiqueta(
                "Seleccione los telefonos activos que desea usar en la prueba.",
                40,
                70,
                780,
                24,
                10,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            Label lblFuente = UiTheme.CrearEtiqueta(
                "Preparando catalogo de telefonos...",
                40,
                96,
                780,
                24,
                9,
                FontStyle.Bold,
                UiTheme.Exito,
                ContentAlignment.MiddleCenter);
            _lblFuente = lblFuente;

            FlowLayoutPanel listaTelefonos = new FlowLayoutPanel
            {
                Location = new Point(40, 138),
                Size = new Size(780, 390),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = UiTheme.Superficie,
                Padding = new Padding(12)
            };
            _listaTelefonos = listaTelefonos;

            listaTelefonos.Controls.Add(UiTheme.CrearEtiqueta(
                "Cargando telefonos disponibles...",
                0,
                0,
                720,
                48,
                10,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter));

            _lblEstado = UiTheme.CrearEtiqueta(
                "Listo para seleccionar.",
                40,
                546,
                780,
                28,
                10,
                FontStyle.Regular,
                UiTheme.TextoSecundario,
                ContentAlignment.MiddleCenter);

            Button btnAbrir = UiTheme.CrearBoton(
                "Abrir telefonos seleccionados",
                180,
                588,
                260,
                44,
                UiTheme.Primario);
            btnAbrir.Click += AbrirTelefonosSeleccionados_Click;

            Button btnAdministracion = UiTheme.CrearBoton(
                "Administracion telefonica",
                460,
                588,
                260,
                44,
                Color.FromArgb(52, 60, 72));
            btnAdministracion.Click += async (_, _) => await AdministracionTelefonicaAsync();

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(lblFuente);
            Controls.Add(listaTelefonos);
            Controls.Add(_lblEstado);
            Controls.Add(btnAbrir);
            Controls.Add(btnAdministracion);

            _pollingTimer.Interval = 6000;
            _pollingTimer.Tick += async (_, _) => await RefrescarCatalogoPollingAsync();
        }

        private CheckBox CrearOpcionTelefono(TelefonoVirtual telefono)
        {
            string estado = telefono.Activo ? "Activo" : "Inactivo";
            string saldo = UiTheme.FormatearSaldo(telefono.TipoServicio, telefono.SaldoDisponible);

            CheckBox opcion = new CheckBox
            {
                Text =
                    $"{telefono.Numero}\n" +
                    $"{telefono.TipoServicio} | {estado} | {telefono.Proveedor} | {telefono.Pais} | {telefono.Nacionalidad}\n" +
                    $"Saldo: {saldo} | " +
                    $"SIM: {UiTheme.ResumirIdentificador(telefono.IdentificadorTarjeta, 22)} | IMEI: {UiTheme.ResumirIdentificador(telefono.IdentificadorDispositivo, 22)}",
                Tag = telefono,
                Size = new Size(736, 88),
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(14, 8, 8, 8),
                BackColor = telefono.Activo
                    ? UiTheme.SuperficieElevada
                    : Color.FromArgb(43, 32, 34),
                ForeColor = telefono.Activo ? UiTheme.Texto : Color.FromArgb(205, 172, 172),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Enabled = telefono.Activo,
                AutoSize = false,
                Cursor = telefono.Activo ? Cursors.Hand : Cursors.No
            };

            if (!telefono.Activo)
            {
                opcion.Text += "\nNo disponible: servicio inactivo.";
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
                _lblEstado.ForeColor = UiTheme.Advertencia;
                return;
            }

            AppConfig.SeleccionarTelefono(seleccionados[0].Id);

            Form1 pantallaPrincipal = new Form1(seleccionados);
            pantallaPrincipal.FormClosed += (s, args) =>
            {
                Show();
                _pollingTimer.Start();
            };
            pantallaPrincipal.Show();

            _pollingTimer.Stop();
            Hide();
        }

        private async Task AdministracionTelefonicaAsync()
        {
            using AdministracionTelefonicaForm form = new AdministracionTelefonicaForm();
            form.ShowDialog(this);

            _pollingTimer.Stop();
            _lblEstado.Text = "Actualizando catalogo despues de administracion...";
            _lblEstado.ForeColor = UiTheme.TextoSecundario;

            await Task.Run(AppConfig.RecargarCatalogoTelefonos);
            RecargarOpciones();

            _lblFuente.Text = AppConfig.FuenteDatosTelefonos;
            _lblFuente.ForeColor = AppConfig.FuenteDatosTelefonos.Contains("reales", StringComparison.OrdinalIgnoreCase)
                ? UiTheme.Exito
                : UiTheme.Advertencia;
            _lblEstado.Text = "Catalogo listo para seleccionar.";
            _lblEstado.ForeColor = UiTheme.TextoSecundario;
            _pollingTimer.Start();
        }

        private async void SeleccionTelefonoForm_Shown(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                WindowState = FormWindowState.Normal;
                BringToFront();
                Activate();
            }));

            _lblEstado.Text = "Cargando catalogo real desde Identificador...";
            _lblEstado.ForeColor = UiTheme.TextoSecundario;

            await Task.Run(AppConfig.RecargarCatalogoTelefonos);

            RecargarOpciones();
            _lblFuente.Text = AppConfig.FuenteDatosTelefonos;
            _lblFuente.ForeColor = AppConfig.FuenteDatosTelefonos.Contains("reales", StringComparison.OrdinalIgnoreCase)
                ? UiTheme.Exito
                : UiTheme.Advertencia;
            _lblEstado.Text = "Catalogo listo para seleccionar.";
            _lblEstado.ForeColor = UiTheme.TextoSecundario;
            _pollingTimer.Start();
        }

        private async Task RefrescarCatalogoPollingAsync()
        {
            if (_actualizandoCatalogo || !Visible)
            {
                return;
            }

            _actualizandoCatalogo = true;
            string[] seleccionados = _opcionesTelefono
                .Where(opcion => opcion.Checked)
                .Select(opcion => opcion.Tag)
                .OfType<TelefonoVirtual>()
                .Select(telefono => telefono.Id)
                .ToArray();

            try
            {
                await Task.Run(AppConfig.RecargarCatalogoTelefonos);
                RecargarOpciones(seleccionados);
                _lblFuente.Text = AppConfig.FuenteDatosTelefonos;
                _lblEstado.Text = $"Catalogo actualizado automaticamente {DateTime.Now:HH:mm:ss}.";
                _lblEstado.ForeColor = UiTheme.TextoSecundario;
            }
            finally
            {
                _actualizandoCatalogo = false;
            }
        }

        private void RecargarOpciones(IEnumerable<string>? idsSeleccionados = null)
        {
            HashSet<string> seleccionados = new(
                idsSeleccionados ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            SuspendLayout();
            _listaTelefonos.SuspendLayout();
            _listaTelefonos.Controls.Clear();
            _opcionesTelefono.Clear();

            foreach (TelefonoVirtual telefono in AppConfig.TelefonosVirtuales)
            {
                CheckBox opcion = CrearOpcionTelefono(telefono);
                opcion.Checked = seleccionados.Contains(telefono.Id) && telefono.Activo;
                _opcionesTelefono.Add(opcion);
                _listaTelefonos.Controls.Add(opcion);
            }

            if (_opcionesTelefono.Count == 0)
            {
                _listaTelefonos.Controls.Add(UiTheme.CrearEtiqueta(
                    "No hay telefonos activos confirmados por el backend.",
                    0,
                    0,
                    720,
                    48,
                    10,
                    FontStyle.Regular,
                    UiTheme.TextoSecundario,
                    ContentAlignment.MiddleCenter));
            }

            _listaTelefonos.ResumeLayout(true);
            ResumeLayout(true);
        }
    }
}
