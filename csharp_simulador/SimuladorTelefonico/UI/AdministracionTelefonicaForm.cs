using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimuladorTelefonico.Config;
using SimuladorTelefonico.Models;
using SimuladorTelefonico.Services;

namespace SimuladorTelefonico.UI
{
    public class AdministracionTelefonicaForm : Form
    {
        private readonly AdministracionTelefonicaService _service = new();
        private readonly CryptoService _cryptoService = new();
        private readonly DataGridView _grid = new();
        private readonly Label _estado = new();
        private readonly TextBox _txtNumero = new();
        private readonly TextBox _txtMonto = new();
        private readonly TextBox _txtNuevoNumero = new();
        private readonly TextBox _txtSim = new();
        private readonly TextBox _txtImei = new();
        private readonly TextBox _txtSaldoInicial = new();
        private readonly ComboBox _cmbTipo = new();
        private readonly ComboBox _cmbProveedor = new();
        private readonly ComboBox _cmbPais = new();
        private readonly CheckBox _chkNuevoActivo = new();
        private readonly CheckBox _chkEstadoSeleccionado = new();
        private readonly TextBox _txtIdentificacionClienteWcf = new();
        private readonly ComboBox _cmbEstadoWcf = new();
        private readonly TextBox _txtDatosWcf = new();
        private List<TelefonoVirtual> _telefonosActuales = new();

        public AdministracionTelefonicaForm()
        {
            Text = "Administracion telefonica";
            UiTheme.ConfigurarVentana(this, new Size(1280, 820), new Size(1180, 760));
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Maximized;
            MaximizeBox = true;
            DoubleBuffered = true;

            ConstruirInterfaz();
            Shown += async (_, _) =>
            {
                GenerarIdentificadoresRegistro();
                await RefrescarCatalogoAsync();
            };
        }

        private void ConstruirInterfaz()
        {
            SuspendLayout();
            Controls.Clear();
            Padding = new Padding(24);

            TableLayoutPanel raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Fondo,
                ColumnCount = 1,
                RowCount = 3
            };
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            Panel header = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Fondo };
            Label titulo = new Label
            {
                Text = "Administracion telefonica",
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Location = new Point(0, 2),
                Size = new Size(620, 38),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label subtitulo = new Label
            {
                Text = "Gestion de telefonos, saldo y estado contra datos reales.",
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 10),
                Location = new Point(2, 40),
                Size = new Size(700, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Button btnVolver = UiTheme.CrearBoton("Volver al selector", 0, 14, 190, 40, Color.FromArgb(52, 60, 72));
            btnVolver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnVolver.Location = new Point(Math.Max(0, ClientSize.Width - 270), 14);
            btnVolver.Click += (_, _) => Close();
            header.Resize += (_, _) => btnVolver.Location = new Point(Math.Max(0, header.Width - 195), 14);
            header.Controls.Add(titulo);
            header.Controls.Add(subtitulo);
            header.Controls.Add(btnVolver);

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Fondo,
                Padding = new Padding(0, 4, 0, 8)
            };
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 286));

            TableLayoutPanel zonaSuperior = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.Fondo,
                Margin = new Padding(0, 0, 0, 14)
            };
            zonaSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74));
            zonaSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26));

            _grid.Dock = DockStyle.Fill;
            _grid.Margin = new Padding(0, 0, 18, 0);
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.BackgroundColor = UiTheme.PanelDatos;
            _grid.BorderStyle = BorderStyle.FixedSingle;
            _grid.RowHeadersVisible = false;
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(32, 38, 48);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _grid.DefaultCellStyle.BackColor = Color.FromArgb(246, 248, 250);
            _grid.DefaultCellStyle.ForeColor = Color.FromArgb(20, 24, 28);
            _grid.DefaultCellStyle.SelectionBackColor = UiTheme.Primario;
            _grid.DefaultCellStyle.SelectionForeColor = Color.White;
            _grid.RowTemplate.Height = 34;
            _grid.CellClick += (_, _) => SincronizarSeleccion();

            FlowLayoutPanel panelAcciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = UiTheme.Fondo,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, 8, 0)
            };

            RoundedPanel seccionRecarga = CrearSeccion("Recargar saldo", 162);
            AgregarCampo(seccionRecarga, "Telefono seleccionado", _txtNumero, 44, 286);
            AgregarCampo(seccionRecarga, "Monto", _txtMonto, 92, 286);
            Button btnRecargar = UiTheme.CrearBoton("Recargar", 22, 132, 286, 34, UiTheme.Primario);
            btnRecargar.Click += async (_, _) => await RecargarAsync();
            seccionRecarga.Controls.Add(btnRecargar);

            RoundedPanel seccionEstado = CrearSeccion("Estado del telefono", 112);
            _chkEstadoSeleccionado.Text = "Telefono activo";
            _chkEstadoSeleccionado.Location = new Point(22, 42);
            _chkEstadoSeleccionado.Size = new Size(286, 26);
            _chkEstadoSeleccionado.ForeColor = UiTheme.Texto;
            _chkEstadoSeleccionado.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _chkEstadoSeleccionado.BackColor = Color.Transparent;
            Button btnCambiarEstado = UiTheme.CrearBoton("Guardar estado", 22, 72, 286, 34, Color.FromArgb(52, 60, 72));
            btnCambiarEstado.Click += async (_, _) => await CambiarEstadoAsync();
            seccionEstado.Controls.Add(_chkEstadoSeleccionado);
            seccionEstado.Controls.Add(btnCambiarEstado);

            RoundedPanel seccionRegistro = CrearSeccion("Registrar telefono", 300);
            AgregarCampo(seccionRegistro, "Numero", _txtNuevoNumero, 44, 286);
            AgregarCampo(seccionRegistro, "SIM", _txtSim, 92, 136);
            AgregarCampo(seccionRegistro, "IMEI", _txtImei, 92, 136, 172);
            AgregarCampo(seccionRegistro, "Saldo inicial", _txtSaldoInicial, 140, 136);
            _txtSim.ReadOnly = true;
            _txtImei.ReadOnly = true;

            _cmbTipo.Items.Clear();
            _cmbProveedor.Items.Clear();
            _cmbPais.Items.Clear();
            _cmbTipo.Items.AddRange(new object[] { "PREPAGO", "POSTPAGO" });
            _cmbTipo.SelectedIndex = 0;
            _cmbProveedor.Items.AddRange(new object[] { "KOLBI", "CLARO", "LIBERTY", "MOVISTAR" });
            _cmbProveedor.SelectedIndex = 0;
            _cmbPais.Items.AddRange(new object[] { "Costa Rica", "Panama", "Nicaragua" });
            _cmbPais.SelectedIndex = 0;

            EstilizarCombo(_cmbTipo);
            EstilizarCombo(_cmbProveedor);
            EstilizarCombo(_cmbPais);
            _cmbTipo.Location = new Point(22, 190);
            _cmbTipo.Size = new Size(91, 28);
            _cmbProveedor.Location = new Point(119, 190);
            _cmbProveedor.Size = new Size(91, 28);
            _cmbPais.Location = new Point(216, 190);
            _cmbPais.Size = new Size(92, 28);

            _chkNuevoActivo.Text = "Registrar como activo";
            _chkNuevoActivo.Checked = true;
            _chkNuevoActivo.Location = new Point(168, 140);
            _chkNuevoActivo.Size = new Size(150, 28);
            _chkNuevoActivo.ForeColor = UiTheme.Texto;
            _chkNuevoActivo.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            _chkNuevoActivo.BackColor = Color.Transparent;

            Button btnGenerarIds = UiTheme.CrearBoton("Generar IDs", 22, 228, 138, 32, Color.FromArgb(52, 60, 72));
            btnGenerarIds.Click += (_, _) => GenerarIdentificadoresRegistro();

            Button btnRegistrar = UiTheme.CrearBoton("Registrar telefono", 170, 228, 138, 32, UiTheme.Primario);
            btnRegistrar.Click += async (_, _) => await RegistrarAsync();

            Button btnRefrescar = UiTheme.CrearBoton("Refrescar", 22, 266, 286, 32, Color.FromArgb(52, 60, 72));
            btnRefrescar.Click += async (_, _) => await RefrescarCatalogoAsync();
            seccionRegistro.Controls.Add(_cmbTipo);
            seccionRegistro.Controls.Add(_cmbProveedor);
            seccionRegistro.Controls.Add(_cmbPais);
            seccionRegistro.Controls.Add(_chkNuevoActivo);
            seccionRegistro.Controls.Add(btnGenerarIds);
            seccionRegistro.Controls.Add(btnRegistrar);
            seccionRegistro.Controls.Add(btnRefrescar);

            RoundedPanel seccionWcf = CrearSeccion("Datos WCF", 268);
            seccionWcf.Dock = DockStyle.Fill;
            seccionWcf.Margin = new Padding(0);

            Label lblWcfSubtitulo = UiTheme.CrearEtiqueta(
                "Valores cifrados listos para pegar en el cliente de prueba WCF.",
                22,
                42,
                520,
                22,
                9,
                FontStyle.Regular,
                UiTheme.TextoSecundario);
            seccionWcf.Controls.Add(lblWcfSubtitulo);

            Panel panelWcfControles = new Panel
            {
                Location = new Point(22, 74),
                Size = new Size(330, 170),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            };

            AgregarCampo(panelWcfControles, "Identificacion cliente", _txtIdentificacionClienteWcf, 0, 144, 0);
            _txtIdentificacionClienteWcf.Text = "118880999";

            Label lblEstadoWcf = UiTheme.CrearEtiqueta(
                "Estado WCF",
                164,
                0,
                144,
                18,
                8,
                FontStyle.Bold,
                UiTheme.TextoSecundario);
            _cmbEstadoWcf.Items.Clear();
            _cmbEstadoWcf.Items.AddRange(new object[] { "activo", "disponible" });
            _cmbEstadoWcf.SelectedIndex = 0;
            EstilizarCombo(_cmbEstadoWcf);
            _cmbEstadoWcf.Location = new Point(164, 20);
            _cmbEstadoWcf.Size = new Size(144, 28);

            Button btnActualizarWcf = UiTheme.CrearBoton("Actualizar WCF", 0, 66, 144, 36, Color.FromArgb(52, 60, 72));
            btnActualizarWcf.Click += (_, _) => ActualizarDatosWcf();

            Button btnCopiarWcf = UiTheme.CrearBoton("Copiar datos", 164, 66, 144, 36, UiTheme.Primario);
            btnCopiarWcf.Click += (_, _) => CopiarDatosWcf();

            Label lblAccionWcf = UiTheme.CrearEtiqueta(
                "activo = activar | disponible = desactivar",
                0,
                116,
                308,
                22,
                8.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario);

            _txtDatosWcf.Location = new Point(372, 74);
            _txtDatosWcf.Size = new Size(720, 170);
            _txtDatosWcf.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txtDatosWcf.Multiline = true;
            _txtDatosWcf.ReadOnly = true;
            _txtDatosWcf.ScrollBars = ScrollBars.Vertical;
            _txtDatosWcf.BackColor = UiTheme.PanelDatos;
            _txtDatosWcf.ForeColor = UiTheme.Texto;
            _txtDatosWcf.BorderStyle = BorderStyle.FixedSingle;
            _txtDatosWcf.Font = new Font("Consolas", 10);
            _txtDatosWcf.WordWrap = false;

            panelWcfControles.Controls.Add(lblEstadoWcf);
            panelWcfControles.Controls.Add(_cmbEstadoWcf);
            panelWcfControles.Controls.Add(btnActualizarWcf);
            panelWcfControles.Controls.Add(btnCopiarWcf);
            panelWcfControles.Controls.Add(lblAccionWcf);
            seccionWcf.Controls.Add(panelWcfControles);
            seccionWcf.Controls.Add(_txtDatosWcf);
            seccionWcf.Resize += (_, _) =>
            {
                int anchoTexto = Math.Max(420, seccionWcf.Width - 394);
                _txtDatosWcf.Size = new Size(anchoTexto, Math.Max(130, seccionWcf.Height - 96));
            };

            panelAcciones.Controls.Add(seccionRecarga);
            panelAcciones.Controls.Add(seccionEstado);
            panelAcciones.Controls.Add(seccionRegistro);

            _estado.Dock = DockStyle.Fill;
            _estado.ForeColor = UiTheme.TextoSecundario;
            _estado.TextAlign = ContentAlignment.MiddleCenter;

            zonaSuperior.Controls.Add(_grid, 0, 0);
            zonaSuperior.Controls.Add(panelAcciones, 1, 0);
            contenido.Controls.Add(zonaSuperior, 0, 0);
            contenido.Controls.Add(seccionWcf, 0, 1);
            raiz.Controls.Add(header, 0, 0);
            raiz.Controls.Add(contenido, 0, 1);
            raiz.Controls.Add(_estado, 0, 2);
            Controls.Add(raiz);

            ResumeLayout(true);
        }

        private RoundedPanel CrearSeccion(string titulo, int alto)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Size = new Size(330, alto),
                Margin = new Padding(0, 0, 0, 14),
                BackColor = UiTheme.SuperficieElevada,
                BorderRadius = 8,
                BorderColor = UiTheme.Borde,
                BorderThickness = 1
            };

            Label lblTitulo = UiTheme.CrearEtiqueta(
                titulo,
                22,
                14,
                286,
                24,
                11.5f,
                FontStyle.Bold,
                UiTheme.Texto);
            panel.Controls.Add(lblTitulo);

            return panel;
        }

        private void AgregarCampo(Control parent, string label, TextBox textBox, int y, int ancho, int x = 26)
        {
            Label lbl = UiTheme.CrearEtiqueta(label, x, y, ancho, 18, 8, FontStyle.Bold, UiTheme.TextoSecundario);
            textBox.Location = new Point(x, y + 20);
            textBox.Size = new Size(ancho, 26);
            EstilizarCampo(textBox);
            parent.Controls.Add(lbl);
            parent.Controls.Add(textBox);
        }

        private static void EstilizarCampo(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(246, 248, 250);
            textBox.ForeColor = Color.FromArgb(22, 26, 32);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 9.25f);
        }

        private static void EstilizarCombo(ComboBox comboBox)
        {
            comboBox.BackColor = Color.FromArgb(246, 248, 250);
            comboBox.ForeColor = Color.FromArgb(22, 26, 32);
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 9.25f);
        }

        private async Task RefrescarCatalogoAsync()
        {
            _estado.Text = "Consultando catalogo real...";
            List<TelefonoVirtual> telefonos = await ConsultarCatalogoConTimeoutAsync();

            if (telefonos.Count > 0)
            {
                AppConfig.ActualizarCatalogoTelefonos(telefonos);
                CargarGrid(telefonos);
                _estado.Text = "Catalogo actualizado desde Python/Java.";
            }
            else
            {
                CargarGrid(AppConfig.TelefonosVirtuales);
                _estado.Text = "No se pudo consultar backend; mostrando catalogo local disponible.";
            }
        }

        private async Task<List<TelefonoVirtual>> ConsultarCatalogoConTimeoutAsync()
        {
            Task<List<TelefonoVirtual>> consulta = _service.ConsultarCatalogoAsync();
            Task timeout = Task.Delay(12000);

            Task completada = await Task.WhenAny(consulta, timeout);
            if (completada != consulta)
            {
                return new List<TelefonoVirtual>();
            }

            try
            {
                return await consulta;
            }
            catch
            {
                return new List<TelefonoVirtual>();
            }
        }

        private void CargarGrid(IEnumerable<TelefonoVirtual> telefonos)
        {
            _telefonosActuales = telefonos.ToList();

            _grid.DataSource = _telefonosActuales
                .Select(t => new
                {
                    t.Id,
                    t.Numero,
                    Tipo = t.TipoServicio,
                    Estado = t.Activo ? "Activo" : "Inactivo",
                    t.Proveedor,
                    t.Pais,
                    Saldo = UiTheme.FormatearSaldo(t.TipoServicio, t.SaldoDisponible)
                })
                .ToList();

            if (_grid.Columns.Count == 0)
            {
                return;
            }

            if (_grid.Columns["Id"] is DataGridViewColumn idColumn)
            {
                idColumn.Visible = false;
            }

            AjustarAnchoColumna("Numero", 120);
            AjustarAnchoColumna("Tipo", 105);
            AjustarAnchoColumna("Estado", 105);
            AjustarAnchoColumna("Proveedor", 145);
            AjustarAnchoColumna("Pais", 130);
            AjustarAnchoColumna("Saldo", 140);

            SincronizarSeleccion();
        }

        private void AjustarAnchoColumna(string nombre, int ancho)
        {
            if (_grid.Columns[nombre] is DataGridViewColumn columna)
            {
                columna.Width = ancho;
            }
        }

        private async Task RecargarAsync()
        {
            if (!decimal.TryParse(_txtMonto.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal monto))
            {
                _estado.Text = "Monto invalido.";
                return;
            }

            var resultado = await _service.RecargarSaldoAsync(_txtNumero.Text.Trim(), monto);
            _estado.Text = resultado.Mensaje;
            await RefrescarCatalogoAsync();
        }

        private async Task RegistrarAsync()
        {
            decimal.TryParse(_txtSaldoInicial.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal saldo);
            string numeroNuevo = _txtNuevoNumero.Text.Trim();

            if (string.IsNullOrWhiteSpace(numeroNuevo))
            {
                _estado.Text = "Digite el numero del telefono.";
                return;
            }

            if (_telefonosActuales.Any(t => t.Numero.Equals(numeroNuevo, StringComparison.OrdinalIgnoreCase)))
            {
                _estado.Text = "Ese numero ya existe. No se permiten telefonos duplicados.";
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtSim.Text) || string.IsNullOrWhiteSpace(_txtImei.Text))
            {
                GenerarIdentificadoresRegistro();
            }

            var resultado = await _service.RegistrarTelefonoAsync(
                numeroNuevo,
                _cmbTipo.Text,
                _cmbProveedor.Text,
                _cmbPais.Text,
                saldo,
                _txtSim.Text.Trim(),
                _txtImei.Text.Trim(),
                _chkNuevoActivo.Checked);

            _estado.Text = resultado.Mensaje;
            await RefrescarCatalogoAsync();
            if (resultado.Exitoso)
            {
                _txtNuevoNumero.Clear();
                _txtSaldoInicial.Clear();
                GenerarIdentificadoresRegistro();
            }
        }

        private async Task CambiarEstadoAsync()
        {
            string numero = _txtNumero.Text.Trim();
            string telefonoId = LeerIdSeleccionado();
            if (string.IsNullOrWhiteSpace(numero))
            {
                _estado.Text = "Seleccione un telefono antes de cambiar estado.";
                return;
            }

            var resultado = await _service.CambiarEstadoAsync(
                telefonoId,
                numero,
                _chkEstadoSeleccionado.Checked);
            _estado.Text = resultado.Mensaje;
            await RefrescarCatalogoAsync();
        }

        private string LeerIdSeleccionado()
        {
            return _grid.CurrentRow?.Cells["Id"]?.Value?.ToString() ?? "";
        }

        private void SincronizarSeleccion()
        {
            if (_grid.CurrentRow?.Cells["Numero"]?.Value is string numero)
            {
                _txtNumero.Text = numero;
            }

            if (_grid.CurrentRow?.Cells["Estado"]?.Value is string estado)
            {
                _chkEstadoSeleccionado.Checked = estado.Equals("Activo", StringComparison.OrdinalIgnoreCase);
                _cmbEstadoWcf.Text = _chkEstadoSeleccionado.Checked ? "activo" : "disponible";
            }

            ActualizarDatosWcf();
        }

        private void GenerarIdentificadoresRegistro()
        {
            string sim;
            string imei;

            do
            {
                sim = "ENC_SIM_" + GenerarDigitos(19);
            }
            while (_telefonosActuales.Any(t => t.IdentificadorTarjeta.Equals(sim, StringComparison.OrdinalIgnoreCase)));

            do
            {
                imei = "ENC_IMEI_" + GenerarDigitos(16);
            }
            while (_telefonosActuales.Any(t => t.IdentificadorDispositivo.Equals(imei, StringComparison.OrdinalIgnoreCase)));

            _txtSim.Text = sim;
            _txtImei.Text = imei;
        }

        private void ActualizarDatosWcf()
        {
            TelefonoVirtual? telefono = ObtenerTelefonoSeleccionado();

            if (telefono == null)
            {
                _txtDatosWcf.Text = "Seleccione un telefono del catalogo.";
                return;
            }

            string identificacionCliente = _txtIdentificacionClienteWcf.Text.Trim();
            if (string.IsNullOrWhiteSpace(identificacionCliente))
            {
                identificacionCliente = "118880999";
            }

            string identificadorTelefono = LimpiarIdentificador(telefono.IdentificadorDispositivo);
            string identificadorTarjeta = LimpiarIdentificador(telefono.IdentificadorTarjeta);
            string estado = string.IsNullOrWhiteSpace(_cmbEstadoWcf.Text)
                ? "activo"
                : _cmbEstadoWcf.Text.Trim().ToLowerInvariant();

            _txtDatosWcf.Text =
                $"NumeroTelefono: {_cryptoService.CifrarDatoSensible(telefono.Numero)}{Environment.NewLine}" +
                $"IdentificadorTelefono: {_cryptoService.CifrarDatoSensible(identificadorTelefono)}{Environment.NewLine}" +
                $"IdentificadorTarjeta: {_cryptoService.CifrarDatoSensible(identificadorTarjeta)}{Environment.NewLine}" +
                $"Tipo: {telefono.TipoServicio}{Environment.NewLine}" +
                $"IdentificacionCliente: {_cryptoService.CifrarDatoSensible(identificacionCliente)}{Environment.NewLine}" +
                $"Estado: {estado}";
        }

        private TelefonoVirtual? ObtenerTelefonoSeleccionado()
        {
            string telefonoId = LeerIdSeleccionado();

            if (!string.IsNullOrWhiteSpace(telefonoId))
            {
                TelefonoVirtual? porId = _telefonosActuales.FirstOrDefault(t =>
                    t.Id.Equals(telefonoId, StringComparison.OrdinalIgnoreCase));

                if (porId != null)
                {
                    return porId;
                }
            }

            if (_grid.CurrentRow?.Cells["Numero"]?.Value is string numero)
            {
                return _telefonosActuales.FirstOrDefault(t =>
                    t.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        private static string LimpiarIdentificador(string valor)
        {
            return (valor ?? string.Empty)
                .Replace("ENC_SIM_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ENC_IMEI_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ENC_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        private void CopiarDatosWcf()
        {
            if (string.IsNullOrWhiteSpace(_txtDatosWcf.Text))
            {
                _estado.Text = "No hay datos WCF para copiar.";
                return;
            }

            Clipboard.SetText(_txtDatosWcf.Text);
            _estado.Text = "Datos WCF copiados al portapapeles.";
        }

        private static string GenerarDigitos(int longitud)
        {
            char[] digitos = new char[longitud];
            byte[] buffer = RandomNumberGenerator.GetBytes(longitud);

            for (int i = 0; i < longitud; i++)
            {
                digitos[i] = (char)('0' + (buffer[i] % 10));
            }

            return new string(digitos);
        }
    }
}
