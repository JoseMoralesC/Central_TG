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
            Padding = new Padding(28);

            TableLayoutPanel raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Fondo,
                ColumnCount = 1,
                RowCount = 3
            };
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

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
                Location = new Point(2, 42),
                Size = new Size(700, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Button btnVolver = UiTheme.CrearBoton("Volver al selector", 0, 12, 190, 42, Color.FromArgb(52, 60, 72));
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
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.Fondo,
                Padding = new Padding(0, 8, 0, 8)
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

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
            _grid.BorderStyle = BorderStyle.None;
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
                Padding = new Padding(0)
            };

            RoundedPanel seccionRecarga = CrearSeccion("Recargar saldo", 178);
            AgregarCampo(seccionRecarga, "Telefono seleccionado", _txtNumero, 48, 310);
            AgregarCampo(seccionRecarga, "Monto", _txtMonto, 100, 310);
            Button btnRecargar = UiTheme.CrearBoton("Recargar", 24, 142, 310, 38, UiTheme.Primario);
            btnRecargar.Click += async (_, _) => await RecargarAsync();
            seccionRecarga.Controls.Add(btnRecargar);

            RoundedPanel seccionEstado = CrearSeccion("Estado del telefono", 126);
            _chkEstadoSeleccionado.Text = "Telefono activo";
            _chkEstadoSeleccionado.Location = new Point(24, 48);
            _chkEstadoSeleccionado.Size = new Size(310, 28);
            _chkEstadoSeleccionado.ForeColor = UiTheme.Texto;
            _chkEstadoSeleccionado.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _chkEstadoSeleccionado.BackColor = Color.Transparent;
            Button btnCambiarEstado = UiTheme.CrearBoton("Guardar estado", 24, 82, 310, 38, Color.FromArgb(52, 60, 72));
            btnCambiarEstado.Click += async (_, _) => await CambiarEstadoAsync();
            seccionEstado.Controls.Add(_chkEstadoSeleccionado);
            seccionEstado.Controls.Add(btnCambiarEstado);

            RoundedPanel seccionRegistro = CrearSeccion("Registrar telefono", 336);
            AgregarCampo(seccionRegistro, "Numero", _txtNuevoNumero, 48, 310);
            AgregarCampo(seccionRegistro, "SIM", _txtSim, 100, 150);
            AgregarCampo(seccionRegistro, "IMEI", _txtImei, 100, 150, 184);
            AgregarCampo(seccionRegistro, "Saldo inicial", _txtSaldoInicial, 152, 150);
            _txtSim.ReadOnly = true;
            _txtImei.ReadOnly = true;

            _cmbTipo.Items.AddRange(new object[] { "PREPAGO", "POSTPAGO" });
            _cmbTipo.SelectedIndex = 0;
            _cmbProveedor.Items.AddRange(new object[] { "KOLBI", "CLARO", "LIBERTY", "MOVISTAR" });
            _cmbProveedor.SelectedIndex = 0;
            _cmbPais.Items.AddRange(new object[] { "Costa Rica", "Panama", "Nicaragua" });
            _cmbPais.SelectedIndex = 0;

            _cmbTipo.Location = new Point(24, 204);
            _cmbTipo.Size = new Size(100, 26);
            _cmbProveedor.Location = new Point(132, 204);
            _cmbProveedor.Size = new Size(100, 26);
            _cmbPais.Location = new Point(240, 204);
            _cmbPais.Size = new Size(94, 26);

            _chkNuevoActivo.Text = "Registrar como activo";
            _chkNuevoActivo.Checked = true;
            _chkNuevoActivo.Location = new Point(184, 152);
            _chkNuevoActivo.Size = new Size(155, 28);
            _chkNuevoActivo.ForeColor = UiTheme.Texto;
            _chkNuevoActivo.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            _chkNuevoActivo.BackColor = Color.Transparent;

            Button btnGenerarIds = UiTheme.CrearBoton("Generar IDs", 24, 244, 150, 38, Color.FromArgb(52, 60, 72));
            btnGenerarIds.Click += (_, _) => GenerarIdentificadoresRegistro();

            Button btnRegistrar = UiTheme.CrearBoton("Registrar telefono", 184, 244, 150, 38, UiTheme.Primario);
            btnRegistrar.Click += async (_, _) => await RegistrarAsync();

            Button btnRefrescar = UiTheme.CrearBoton("Refrescar", 24, 288, 310, 38, Color.FromArgb(52, 60, 72));
            btnRefrescar.Click += async (_, _) => await RefrescarCatalogoAsync();
            seccionRegistro.Controls.Add(_cmbTipo);
            seccionRegistro.Controls.Add(_cmbProveedor);
            seccionRegistro.Controls.Add(_cmbPais);
            seccionRegistro.Controls.Add(_chkNuevoActivo);
            seccionRegistro.Controls.Add(btnGenerarIds);
            seccionRegistro.Controls.Add(btnRegistrar);
            seccionRegistro.Controls.Add(btnRefrescar);

            panelAcciones.Controls.Add(seccionRecarga);
            panelAcciones.Controls.Add(seccionEstado);
            panelAcciones.Controls.Add(seccionRegistro);

            _estado.Dock = DockStyle.Fill;
            _estado.ForeColor = UiTheme.TextoSecundario;
            _estado.TextAlign = ContentAlignment.MiddleCenter;

            contenido.Controls.Add(_grid, 0, 0);
            contenido.Controls.Add(panelAcciones, 1, 0);
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
                Size = new Size(360, alto),
                Margin = new Padding(0, 0, 0, 14),
                BackColor = UiTheme.SuperficieElevada,
                BorderRadius = 18,
                BorderColor = UiTheme.Borde,
                BorderThickness = 1
            };

            Label lblTitulo = UiTheme.CrearEtiqueta(
                titulo,
                24,
                16,
                310,
                24,
                12,
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
            parent.Controls.Add(lbl);
            parent.Controls.Add(textBox);
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
            }
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
                imei = "ENC_IMEI_" + GenerarDigitos(15);
            }
            while (_telefonosActuales.Any(t => t.IdentificadorDispositivo.Equals(imei, StringComparison.OrdinalIgnoreCase)));

            _txtSim.Text = sim;
            _txtImei.Text = imei;
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
