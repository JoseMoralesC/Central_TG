using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        private readonly AutenticacionCryptoService _authCryptoService = new();
        private readonly MongoUsuariosConsultaService _mongoUsuariosService = new();
        private readonly DataGridView _grid = new();
        private readonly DataGridView _gridUsuarios = new();
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
        private readonly TextBox _txtRegistroWcfNumero = new();
        private readonly TextBox _txtRegistroWcfIdentificadorTelefono = new();
        private readonly TextBox _txtRegistroWcfIdentificadorTarjeta = new();
        private readonly ComboBox _cmbRegistroWcfTipo = new();
        private readonly ComboBox _cmbRegistroWcfEstado = new();
        private readonly ComboBox _cmbEstadoWcf = new();
        private readonly TextBox _txtDatosWcf = new();
        private readonly TextBox _txtAuthIdentificacion = new();
        private readonly TextBox _txtAuthNombre = new();
        private readonly TextBox _txtAuthPrimerApellido = new();
        private readonly TextBox _txtAuthSegundoApellido = new();
        private readonly TextBox _txtAuthCorreo = new();
        private readonly TextBox _txtAuthUsuario = new();
        private readonly TextBox _txtAuthContrasena = new();
        private readonly ComboBox _cmbAuthEstado = new();
        private readonly ComboBox _cmbAuthTipo = new();
        private readonly TextBox _txtDatosUsuarioWcf = new();
        private readonly TextBox _txtFactFechaCalculo = new();
        private readonly TextBox _txtFactFechaMaximaPago = new();
        private readonly TextBox _txtDatosFacturacionWcf = new();
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
                GenerarDatosRegistroWcf();
                ActualizarDatosWcf();
            };
        }

        private void ConstruirInterfaz()
        {
            SuspendLayout();
            Controls.Clear();
            Padding = new Padding(20);

            TableLayoutPanel raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Fondo,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0)
            };
            raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            TableLayoutPanel header = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = UiTheme.Fondo,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 12),
                Margin = new Padding(0)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            FlowLayoutPanel bloqueTitulo = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(0)
            };
            Label titulo = new Label
            {
                Text = "Administracion telefonica",
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2)
            };
            Label subtitulo = new Label
            {
                Text = "Gestion de telefonos, saldo y estado contra datos reales.",
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                AutoEllipsis = true,
                Margin = new Padding(2, 0, 0, 0)
            };
            Button btnVolver = UiTheme.CrearBoton("Volver al selector", 0, 0, 190, 38, Color.FromArgb(52, 60, 72));
            ConfigurarBotonSecundario(btnVolver);
            btnVolver.Width = 190;
            btnVolver.Dock = DockStyle.None;
            btnVolver.Margin = new Padding(0, 6, 0, 0);
            btnVolver.Click += (_, _) => Close();

            FlowLayoutPanel panelVolver = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            panelVolver.Controls.Add(btnVolver);

            bloqueTitulo.Controls.Add(titulo);
            bloqueTitulo.Controls.Add(subtitulo);
            header.Controls.Add(bloqueTitulo, 0, 0);
            header.Controls.Add(panelVolver, 1, 0);

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                BackColor = UiTheme.Fondo,
                Padding = new Padding(0, 4, 0, 8)
            };
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _grid.Dock = DockStyle.Fill;
            _grid.Margin = new Padding(0);
            ConfigurarGrid(_grid);
            _grid.CellClick += (_, _) => SincronizarSeleccion();

            TableLayoutPanel zonaAdministracion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Fondo,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(12)
            };
            zonaAdministracion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74));
            zonaAdministracion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26));

            FlowLayoutPanel panelAcciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = UiTheme.Fondo,
                Margin = new Padding(12, 0, 0, 0),
                Padding = new Padding(0)
            };
            panelAcciones.Resize += (_, _) => AjustarAnchoTarjetas(panelAcciones);

            FlowLayoutPanel seccionRecarga = CrearTarjeta("Recargar saldo");
            AgregarCampo(seccionRecarga, "Telefono seleccionado", _txtNumero);
            AgregarCampo(seccionRecarga, "Monto", _txtMonto);
            Button btnRecargar = CrearBotonPrimario("Recargar");
            btnRecargar.Click += async (_, _) => await RecargarAsync();
            seccionRecarga.Controls.Add(btnRecargar);

            FlowLayoutPanel seccionEstado = CrearTarjeta("Estado del telefono");
            _chkEstadoSeleccionado.Text = "Telefono activo";
            _chkEstadoSeleccionado.Dock = DockStyle.Top;
            _chkEstadoSeleccionado.Height = 30;
            _chkEstadoSeleccionado.Margin = new Padding(0, 0, 0, 10);
            _chkEstadoSeleccionado.ForeColor = UiTheme.Texto;
            _chkEstadoSeleccionado.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _chkEstadoSeleccionado.BackColor = Color.Transparent;
            Button btnCambiarEstado = CrearBotonSecundario("Guardar estado");
            btnCambiarEstado.Click += async (_, _) => await CambiarEstadoAsync();
            seccionEstado.Controls.Add(_chkEstadoSeleccionado);
            seccionEstado.Controls.Add(btnCambiarEstado);

            FlowLayoutPanel seccionRegistro = CrearTarjeta("Registrar telefono");
            AgregarCampo(seccionRegistro, "Numero", _txtNuevoNumero);
            TableLayoutPanel filaIdentificadores = CrearTablaColumnas(2);
            filaIdentificadores.Controls.Add(CrearCampoConEtiqueta("SIM", _txtSim), 0, 0);
            filaIdentificadores.Controls.Add(CrearCampoConEtiqueta("IMEI", _txtImei), 1, 0);
            seccionRegistro.Controls.Add(filaIdentificadores);
            AgregarCampo(seccionRegistro, "Saldo inicial", _txtSaldoInicial);
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

            _chkNuevoActivo.Text = "Registrar como activo";
            _chkNuevoActivo.Checked = true;
            _chkNuevoActivo.Dock = DockStyle.Top;
            _chkNuevoActivo.Height = 30;
            _chkNuevoActivo.Margin = new Padding(0, 0, 0, 10);
            _chkNuevoActivo.ForeColor = UiTheme.Texto;
            _chkNuevoActivo.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            _chkNuevoActivo.BackColor = Color.Transparent;

            TableLayoutPanel filaCombos = CrearTablaColumnas(3);
            filaCombos.Controls.Add(CrearCampoConEtiqueta("Tipo", _cmbTipo), 0, 0);
            filaCombos.Controls.Add(CrearCampoConEtiqueta("Proveedor", _cmbProveedor), 1, 0);
            filaCombos.Controls.Add(CrearCampoConEtiqueta("Pais", _cmbPais), 2, 0);

            Button btnGenerarIds = CrearBotonSecundario("Generar IDs");
            btnGenerarIds.Click += (_, _) => GenerarIdentificadoresRegistro();

            Button btnRegistrar = CrearBotonPrimario("Registrar telefono");
            btnRegistrar.Click += async (_, _) => await RegistrarAsync();

            Button btnRefrescar = CrearBotonSecundario("Refrescar");
            btnRefrescar.Click += async (_, _) => await RefrescarCatalogoAsync();

            TableLayoutPanel filaBotonesRegistro = CrearTablaColumnas(2);
            filaBotonesRegistro.Controls.Add(btnGenerarIds, 0, 0);
            filaBotonesRegistro.Controls.Add(btnRegistrar, 1, 0);

            seccionRegistro.Controls.Add(_chkNuevoActivo);
            seccionRegistro.Controls.Add(filaCombos);
            seccionRegistro.Controls.Add(filaBotonesRegistro);
            seccionRegistro.Controls.Add(btnRefrescar);

            panelAcciones.Controls.Add(seccionRecarga);
            panelAcciones.Controls.Add(seccionEstado);
            panelAcciones.Controls.Add(seccionRegistro);

            zonaAdministracion.Controls.Add(_grid, 0, 0);
            zonaAdministracion.Controls.Add(panelAcciones, 1, 0);

            TabControl panelPruebas = CrearPanelPruebas(zonaAdministracion);

            _estado.Dock = DockStyle.Fill;
            _estado.ForeColor = UiTheme.TextoSecundario;
            _estado.TextAlign = ContentAlignment.MiddleCenter;
            _estado.AutoSize = true;
            _estado.Padding = new Padding(0, 8, 0, 0);
            _estado.MinimumSize = new Size(0, 30);

            contenido.Controls.Add(panelPruebas, 0, 0);
            raiz.Controls.Add(header, 0, 0);
            raiz.Controls.Add(contenido, 0, 1);
            raiz.Controls.Add(_estado, 0, 2);
            Controls.Add(raiz);

            ResumeLayout(true);
        }

        private FlowLayoutPanel CrearTarjeta(string titulo)
        {
            FlowLayoutPanel panel = new()
            {
                Width = 330,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(12),
                BackColor = UiTheme.SuperficieElevada,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitulo = new()
            {
                Text = titulo,
                Height = 28,
                Width = 300,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10)
            };
            panel.Controls.Add(lblTitulo);
            panel.Resize += (_, _) => AjustarAnchoTarjetas(panel);

            return panel;
        }

        private void AgregarCampo(Control parent, string label, TextBox textBox)
        {
            parent.Controls.Add(CrearCampoConEtiqueta(label, textBox));
        }

        private Control CrearCampoConEtiqueta(string label, Control campo)
        {
            TableLayoutPanel contenedor = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                MinimumSize = new Size(0, 54)
            };
            contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label etiqueta = new()
            {
                Text = label,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextoSecundario,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 4)
            };

            ConfigurarCampo(campo);
            campo.Dock = DockStyle.Top;
            campo.Height = 30;
            campo.MinimumSize = new Size(80, 30);
            campo.Margin = new Padding(0);

            contenedor.Controls.Add(etiqueta, 0, 0);
            contenedor.Controls.Add(campo, 0, 1);
            return contenedor;
        }

        private static void ConfigurarCampo(Control campo)
        {
            campo.BackColor = Color.FromArgb(246, 248, 250);
            campo.ForeColor = Color.FromArgb(22, 26, 32);
            campo.Font = new Font("Segoe UI", 9.25f);

            if (campo is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (campo is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private static void EstilizarCombo(ComboBox comboBox) => ConfigurarCampo(comboBox);

        private static Button CrearBotonPrimario(string texto)
        {
            Button boton = UiTheme.CrearBoton(texto, 0, 0, 160, 36, UiTheme.Primario);
            ConfigurarBotonPrimario(boton);
            return boton;
        }

        private static Button CrearBotonSecundario(string texto)
        {
            Button boton = UiTheme.CrearBoton(texto, 0, 0, 160, 36, Color.FromArgb(52, 60, 72));
            ConfigurarBotonSecundario(boton);
            return boton;
        }

        private static void ConfigurarBotonPrimario(Button boton)
        {
            boton.Dock = DockStyle.Top;
            boton.Height = 36;
            boton.MinimumSize = new Size(120, 36);
            boton.Margin = new Padding(0, 0, 0, 10);
            boton.Font = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            boton.ForeColor = Color.White;
            boton.BackColor = UiTheme.Primario;
        }

        private static void ConfigurarBotonSecundario(Button boton)
        {
            boton.Dock = DockStyle.Top;
            boton.Height = 36;
            boton.MinimumSize = new Size(120, 36);
            boton.Margin = new Padding(0, 0, 0, 10);
            boton.Font = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            boton.ForeColor = Color.White;
            boton.BackColor = Color.FromArgb(52, 60, 72);
        }

        private static TableLayoutPanel CrearTablaColumnas(int columnas)
        {
            TableLayoutPanel tabla = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = columnas,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };

            for (int i = 0; i < columnas; i++)
            {
                tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columnas));
            }

            return tabla;
        }

        private static void ConfigurarGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.BackgroundColor = UiTheme.PanelDatos;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.RowHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(32, 38, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(246, 248, 250);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(20, 24, 28);
            grid.DefaultCellStyle.SelectionBackColor = UiTheme.Primario;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.RowTemplate.Height = 30;
            grid.MinimumSize = new Size(320, 180);
        }

        private static void AjustarAnchoTarjetas(FlowLayoutPanel panel)
        {
            int ancho = Math.Max(300, panel.ClientSize.Width - panel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 2);
            foreach (Control control in panel.Controls)
            {
                control.Width = ancho;
            }
        }

        private TabControl CrearPanelPruebas(Control administracionTelefonica)
        {
            TabControl tabs = new()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.25f),
                Margin = new Padding(0)
            };

            tabs.TabPages.Add(CrearTabAdministracionTelefonica(administracionTelefonica));
            tabs.TabPages.Add(CrearTabTelefoniaWcf());
            tabs.TabPages.Add(CrearTabUsuariosWcf());
            tabs.TabPages.Add(CrearTabFacturacionWcf());

            return tabs;
        }

        private TabPage CrearTabAdministracionTelefonica(Control contenido)
        {
            TabPage tab = CrearTab("Administracion telefonica");
            contenido.Dock = DockStyle.Fill;
            contenido.Margin = new Padding(16);
            tab.Controls.Add(contenido);
            return tab;
        }

        private TabPage CrearTabTelefoniaWcf()
        {
            TabPage tab = CrearTab("Datos WCF telefonia");
            TableLayoutPanel layout = CrearLayoutDosColumnas(340);
            FlowLayoutPanel controles = CrearPanelFormulario();

            controles.Controls.Add(CrearTextoInformativo("Valores de apoyo para RegistrarLinea, ConsultarSaldo y ActivarDesactivarLinea."));
            controles.Controls.Add(CrearTextoInformativo("Registro de linea nueva"));
            AgregarCampo(controles, "Numero nuevo", _txtRegistroWcfNumero);
            AgregarCampo(controles, "Identificador telefono (16 digitos)", _txtRegistroWcfIdentificadorTelefono);
            AgregarCampo(controles, "Identificador tarjeta (19 digitos)", _txtRegistroWcfIdentificadorTarjeta);

            _cmbRegistroWcfTipo.Items.Clear();
            _cmbRegistroWcfTipo.Items.AddRange(new object[] { "PREPAGO", "POSTPAGO" });
            _cmbRegistroWcfTipo.SelectedIndex = 0;
            EstilizarCombo(_cmbRegistroWcfTipo);

            _cmbRegistroWcfEstado.Items.Clear();
            _cmbRegistroWcfEstado.Items.AddRange(new object[] { "inactivo", "activo" });
            _cmbRegistroWcfEstado.SelectedIndex = 0;
            EstilizarCombo(_cmbRegistroWcfEstado);

            TableLayoutPanel filaRegistro = CrearTablaColumnas(2);
            filaRegistro.Controls.Add(CrearCampoConEtiqueta("Tipo", _cmbRegistroWcfTipo), 0, 0);
            filaRegistro.Controls.Add(CrearCampoConEtiqueta("Estado registro", _cmbRegistroWcfEstado), 1, 0);
            controles.Controls.Add(filaRegistro);

            Button btnGenerarLineaNueva = CrearBotonSecundario("Generar linea nueva");
            btnGenerarLineaNueva.Click += (_, _) =>
            {
                GenerarDatosRegistroWcf();
                ActualizarDatosWcf();
            };
            controles.Controls.Add(btnGenerarLineaNueva);

            controles.Controls.Add(CrearTextoInformativo("Linea existente seleccionada"));
            AgregarCampo(controles, "Identificacion cliente", _txtIdentificacionClienteWcf);
            _txtIdentificacionClienteWcf.Text = "118880999";

            _cmbEstadoWcf.Items.Clear();
            _cmbEstadoWcf.Items.AddRange(new object[] { "activo", "inactivo" });
            _cmbEstadoWcf.SelectedIndex = 0;
            EstilizarCombo(_cmbEstadoWcf);
            controles.Controls.Add(CrearCampoConEtiqueta("Estado WCF", _cmbEstadoWcf));

            Button btnActualizarWcf = CrearBotonSecundario("Actualizar WCF");
            btnActualizarWcf.Click += (_, _) => ActualizarDatosWcf();
            controles.Controls.Add(btnActualizarWcf);

            Button btnCopiarWcf = CrearBotonPrimario("Copiar datos");
            btnCopiarWcf.Click += (_, _) => CopiarDatosWcf();
            controles.Controls.Add(btnCopiarWcf);

            controles.Controls.Add(CrearTextoInformativo("activo = activar | inactivo = desactivar"));
            controles.Resize += (_, _) => AjustarAnchoTarjetas(controles);

            ConfigurarAreaResultados(_txtDatosWcf);
            layout.Controls.Add(controles, 0, 0);
            layout.Controls.Add(_txtDatosWcf, 1, 0);
            tab.Controls.Add(layout);

            return tab;
        }

        private TabPage CrearTabUsuariosWcf()
        {
            TabPage tab = CrearTab("Usuarios Mongo / WCF");
            TableLayoutPanel layout = CrearLayoutDosColumnas(420);
            FlowLayoutPanel campos = CrearPanelFormulario();
            campos.Controls.Add(CrearTextoInformativo("Credenciales cifradas y datos exactos para WS_Autenticacion."));

            AgregarCampo(campos, "Identificacion", _txtAuthIdentificacion);

            TableLayoutPanel filaNombres = CrearTablaColumnas(3);
            filaNombres.Controls.Add(CrearCampoConEtiqueta("Nombre", _txtAuthNombre), 0, 0);
            filaNombres.Controls.Add(CrearCampoConEtiqueta("Primer apellido", _txtAuthPrimerApellido), 1, 0);
            filaNombres.Controls.Add(CrearCampoConEtiqueta("Segundo apellido", _txtAuthSegundoApellido), 2, 0);
            campos.Controls.Add(filaNombres);

            AgregarCampo(campos, "Correo electronico", _txtAuthCorreo);

            TableLayoutPanel filaCredenciales = CrearTablaColumnas(2);
            filaCredenciales.Controls.Add(CrearCampoConEtiqueta("Usuario", _txtAuthUsuario), 0, 0);
            filaCredenciales.Controls.Add(CrearCampoConEtiqueta("Contrasena", _txtAuthContrasena), 1, 0);
            campos.Controls.Add(filaCredenciales);

            _txtAuthIdentificacion.Text = "118880999";
            _txtAuthNombre.Text = "Jose";
            _txtAuthPrimerApellido.Text = "Morales";
            _txtAuthSegundoApellido.Text = "Calderon";
            _txtAuthCorreo.Text = "jose@example.com";
            _txtAuthUsuario.Text = "jose";
            _txtAuthContrasena.Text = "ClavePrueba1!*";

            _cmbAuthEstado.Items.AddRange(new object[] { "activo", "inactivo" });
            _cmbAuthEstado.SelectedIndex = 0;
            EstilizarCombo(_cmbAuthEstado);
            _cmbAuthTipo.Items.AddRange(new object[] { "1", "2" });
            _cmbAuthTipo.SelectedIndex = 0;
            EstilizarCombo(_cmbAuthTipo);

            TableLayoutPanel filaEstado = CrearTablaColumnas(2);
            filaEstado.Controls.Add(CrearCampoConEtiqueta("Estado", _cmbAuthEstado), 0, 0);
            filaEstado.Controls.Add(CrearCampoConEtiqueta("Tipo", _cmbAuthTipo), 1, 0);
            campos.Controls.Add(filaEstado);

            TableLayoutPanel filaBotones = CrearTablaColumnas(2);
            Button btnGenerar = CrearBotonSecundario("Generar credenciales");
            btnGenerar.Click += (_, _) => ActualizarDatosUsuarioWcf();
            Button btnCopiar = CrearBotonPrimario("Copiar");
            btnCopiar.Click += (_, _) => CopiarTexto(_txtDatosUsuarioWcf, "Datos WCF de usuario copiados.");
            filaBotones.Controls.Add(btnGenerar, 0, 0);
            filaBotones.Controls.Add(btnCopiar, 1, 0);
            campos.Controls.Add(filaBotones);

            Button btnMongo = CrearBotonSecundario("Consultar Mongo");
            btnMongo.Click += async (_, _) => await ConsultarUsuariosMongoAsync();
            campos.Controls.Add(btnMongo);
            campos.Resize += (_, _) => AjustarAnchoTarjetas(campos);

            SplitContainer resultados = new()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 360,
                SplitterWidth = 8,
                BackColor = UiTheme.SuperficieElevada,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            ConfigurarAreaResultados(_txtDatosUsuarioWcf);
            PrepararGridUsuarios();
            resultados.Panel1.Controls.Add(_txtDatosUsuarioWcf);
            resultados.Panel2.Controls.Add(_gridUsuarios);

            layout.Controls.Add(campos, 0, 0);
            layout.Controls.Add(resultados, 1, 0);
            tab.Controls.Add(layout);

            ActualizarDatosUsuarioWcf();

            return tab;
        }

        private TabPage CrearTabFacturacionWcf()
        {
            TabPage tab = CrearTab("Facturacion WCF");
            TableLayoutPanel layout = CrearLayoutDosColumnas(340);
            FlowLayoutPanel campos = CrearPanelFormulario();
            campos.Controls.Add(CrearTextoInformativo("Fechas listas para CalcularFacturacion en formato yyyy-MM-dd."));

            AgregarCampo(campos, "Fecha de calculo", _txtFactFechaCalculo);
            AgregarCampo(campos, "Fecha maxima de pago", _txtFactFechaMaximaPago);
            _txtFactFechaCalculo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            _txtFactFechaMaximaPago.Text = DateTime.Today.AddDays(15).ToString("yyyy-MM-dd");

            Button btnGenerar = CrearBotonSecundario("Generar WCF");
            btnGenerar.Click += (_, _) => ActualizarDatosFacturacionWcf();
            campos.Controls.Add(btnGenerar);

            Button btnCopiar = CrearBotonPrimario("Copiar datos");
            btnCopiar.Click += (_, _) => CopiarTexto(_txtDatosFacturacionWcf, "Datos WCF de facturacion copiados.");
            campos.Controls.Add(btnCopiar);

            campos.Controls.Add(CrearTextoInformativo("La fecha maxima debe ser igual o posterior a la fecha de calculo."));
            campos.Resize += (_, _) => AjustarAnchoTarjetas(campos);

            ConfigurarAreaResultados(_txtDatosFacturacionWcf);
            layout.Controls.Add(campos, 0, 0);
            layout.Controls.Add(_txtDatosFacturacionWcf, 1, 0);
            tab.Controls.Add(layout);

            ActualizarDatosFacturacionWcf();

            return tab;
        }

        private TabPage CrearTab(string titulo)
        {
            return new TabPage(titulo)
            {
                BackColor = UiTheme.SuperficieElevada,
                ForeColor = UiTheme.Texto,
                Padding = new Padding(12)
            };
        }

        private static TableLayoutPanel CrearLayoutDosColumnas(int anchoFormulario)
        {
            TableLayoutPanel layout = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.SuperficieElevada,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, anchoFormulario));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            return layout;
        }

        private static FlowLayoutPanel CrearPanelFormulario()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = UiTheme.SuperficieElevada,
                Padding = new Padding(0, 0, 12, 0),
                Margin = new Padding(0, 0, 12, 0)
            };
        }

        private static Label CrearTextoInformativo(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = false,
                Height = 44,
                Width = 300,
                ForeColor = UiTheme.TextoSecundario,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9),
                AutoEllipsis = true,
                Margin = new Padding(0, 0, 0, 12)
            };
        }

        private static void ConfigurarAreaResultados(TextBox textBox)
        {
            textBox.Dock = DockStyle.Fill;
            textBox.Margin = new Padding(0);
            textBox.MinimumSize = new Size(360, 180);
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.BackColor = UiTheme.PanelDatos;
            textBox.ForeColor = UiTheme.Texto;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Consolas", 9.25f);
            textBox.WordWrap = false;
        }

        private void PrepararGridUsuarios()
        {
            _gridUsuarios.Dock = DockStyle.Fill;
            _gridUsuarios.Margin = new Padding(0);
            ConfigurarGrid(_gridUsuarios);
            _gridUsuarios.CellClick += (_, _) => SincronizarUsuarioSeleccionado();
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
                _cmbEstadoWcf.Text = _chkEstadoSeleccionado.Checked ? "activo" : "inactivo";
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

        private void GenerarDatosRegistroWcf()
        {
            string numero;
            do
            {
                numero = "6" + GenerarDigitos(7);
            }
            while (_telefonosActuales.Any(t => t.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase)));

            string identificadorTelefono;
            do
            {
                identificadorTelefono = GenerarDigitos(16);
            }
            while (_telefonosActuales.Any(t =>
                LimpiarIdentificador(t.IdentificadorDispositivo).Equals(identificadorTelefono, StringComparison.OrdinalIgnoreCase)));

            string identificadorTarjeta;
            do
            {
                identificadorTarjeta = GenerarDigitos(19);
            }
            while (_telefonosActuales.Any(t =>
                LimpiarIdentificador(t.IdentificadorTarjeta).Equals(identificadorTarjeta, StringComparison.OrdinalIgnoreCase)));

            _txtRegistroWcfNumero.Text = numero;
            _txtRegistroWcfIdentificadorTelefono.Text = identificadorTelefono;
            _txtRegistroWcfIdentificadorTarjeta.Text = identificadorTarjeta;

            if (string.IsNullOrWhiteSpace(_cmbRegistroWcfTipo.Text))
            {
                _cmbRegistroWcfTipo.Text = "PREPAGO";
            }

            if (string.IsNullOrWhiteSpace(_cmbRegistroWcfEstado.Text))
            {
                _cmbRegistroWcfEstado.Text = "inactivo";
            }
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

            if (string.IsNullOrWhiteSpace(_txtRegistroWcfNumero.Text)
                || string.IsNullOrWhiteSpace(_txtRegistroWcfIdentificadorTelefono.Text)
                || string.IsNullOrWhiteSpace(_txtRegistroWcfIdentificadorTarjeta.Text))
            {
                GenerarDatosRegistroWcf();
            }

            string identificadorTelefono = LimpiarIdentificador(telefono.IdentificadorDispositivo);
            string identificadorTarjeta = LimpiarIdentificador(telefono.IdentificadorTarjeta);
            string estado = string.IsNullOrWhiteSpace(_cmbEstadoWcf.Text)
                ? "activo"
                : _cmbEstadoWcf.Text.Trim().ToLowerInvariant();
            string registroNumero = _txtRegistroWcfNumero.Text.Trim();
            string registroIdentificadorTelefono = SoloDigitos(_txtRegistroWcfIdentificadorTelefono.Text);
            string registroIdentificadorTarjeta = SoloDigitos(_txtRegistroWcfIdentificadorTarjeta.Text);
            string registroTipo = string.IsNullOrWhiteSpace(_cmbRegistroWcfTipo.Text)
                ? "PREPAGO"
                : _cmbRegistroWcfTipo.Text.Trim().ToUpperInvariant();
            string registroEstado = string.IsNullOrWhiteSpace(_cmbRegistroWcfEstado.Text)
                ? "inactivo"
                : _cmbRegistroWcfEstado.Text.Trim().ToLowerInvariant();

            string numeroCifrado = _cryptoService.CifrarDatoSensible(telefono.Numero);
            string identificadorTelefonoCifrado = _cryptoService.CifrarDatoSensible(identificadorTelefono);
            string identificadorTarjetaCifrado = _cryptoService.CifrarDatoSensible(identificadorTarjeta);
            string identificacionClienteCifrada = _cryptoService.CifrarDatoSensible(identificacionCliente);

            StringBuilder datos = new();
            datos.AppendLine("WS_PROVEEDOR2 / ActivarDesactivarLinea (cifrado)");
            datos.AppendLine($"NumeroTelefono: {numeroCifrado}");
            datos.AppendLine($"IdentificadorTelefono: {identificadorTelefonoCifrado}");
            datos.AppendLine($"IdentificadorTarjeta: {identificadorTarjetaCifrado}");
            datos.AppendLine($"Tipo: {telefono.TipoServicio}");
            datos.AppendLine($"IdentificacionCliente: {identificacionClienteCifrada}");
            datos.AppendLine($"Estado: {estado}");
            datos.AppendLine();
            datos.AppendLine("WS_PROVEEDOR1 / RegistrarLinea (linea nueva, plano; el WCF cifra internamente)");
            datos.AppendLine($"NumeroTelefono: {registroNumero}");
            datos.AppendLine($"IdentificadorTelefono: {registroIdentificadorTelefono}");
            datos.AppendLine($"IdentificadorTarjeta: {registroIdentificadorTarjeta}");
            datos.AppendLine($"Tipo: {registroTipo}");
            datos.AppendLine($"Estado: {registroEstado}");
            datos.AppendLine($"Validacion local: telefono nuevo={registroNumero.Length == 8 && registroNumero.All(char.IsDigit)}, idTelefono16={registroIdentificadorTelefono.Length == 16}, idTarjeta19={registroIdentificadorTarjeta.Length == 19}");
            datos.AppendLine();
            datos.AppendLine("WS_IDENTIFICADOR1 / ConsultarSaldo WEB (plano; el WCF cifra internamente)");
            datos.AppendLine($"NumeroTelefono: {telefono.Numero}");
            datos.AppendLine("Origen: WEB");
            datos.AppendLine("TipoTransaccion: CONSULTA_SALDO");
            datos.AppendLine();
            datos.AppendLine("WS_IDENTIFICADOR1 / ConsultarSaldo TELEFONO (plano; el WCF cifra internamente)");
            datos.AppendLine($"NumeroTelefono: {telefono.Numero}");
            datos.AppendLine("Origen: TELEFONO");
            datos.AppendLine("TipoTransaccion: CONSULTA_SALDO");
            datos.AppendLine($"IdentificadorTelefono: {identificadorTelefono}");
            datos.AppendLine($"IdentificadorTarjeta: {identificadorTarjeta}");
            datos.AppendLine("Pais: Costa Rica");
            datos.AppendLine("Provincia: San Jose");
            datos.AppendLine("Latitud: 9.9281");
            datos.AppendLine("Longitud: -84.0907");

            _txtDatosWcf.Text = datos.ToString();
        }

        private void ActualizarDatosUsuarioWcf()
        {
            try
            {
                string usuarioCifrado = _authCryptoService.Cifrar(_txtAuthUsuario.Text.Trim());
                string contrasenaCifrada = _authCryptoService.Cifrar(_txtAuthContrasena.Text);
                string tipo = string.IsNullOrWhiteSpace(_cmbAuthTipo.Text) ? "1" : _cmbAuthTipo.Text.Trim();
                string estado = string.IsNullOrWhiteSpace(_cmbAuthEstado.Text)
                    ? "activo"
                    : _cmbAuthEstado.Text.Trim().ToLowerInvariant();

                StringBuilder datos = new();
                datos.AppendLine("WS_AUTENTICACION2 / CrearUsuario");
                datos.AppendLine($"identificacion: {_txtAuthIdentificacion.Text.Trim()}");
                datos.AppendLine($"nombre: {_txtAuthNombre.Text.Trim()}");
                datos.AppendLine($"primerApellido: {_txtAuthPrimerApellido.Text.Trim()}");
                datos.AppendLine($"segundoApellido: {_txtAuthSegundoApellido.Text.Trim()}");
                datos.AppendLine($"correoElectronico: {_txtAuthCorreo.Text.Trim()}");
                datos.AppendLine($"usuarioEncriptado: {usuarioCifrado}");
                datos.AppendLine($"contrasenaEncriptada: {contrasenaCifrada}");
                datos.AppendLine($"estado: {estado}");
                datos.AppendLine($"tipo: {tipo}");
                datos.AppendLine();
                datos.AppendLine("WS_AUTENTICACION1 / AutenticarUsuario");
                datos.AppendLine($"usuarioEncriptado: {usuarioCifrado}");
                datos.AppendLine($"contrasenaEncriptada: {contrasenaCifrada}");
                datos.AppendLine($"tipo: {tipo}");
                datos.AppendLine();
                datos.AppendLine("WS_AUTENTICACION2 / ModificarUsuario");
                datos.AppendLine($"identificacion: {_txtAuthIdentificacion.Text.Trim()}");
                datos.AppendLine($"nombre: {_txtAuthNombre.Text.Trim()}");
                datos.AppendLine($"primerApellido: {_txtAuthPrimerApellido.Text.Trim()}");
                datos.AppendLine($"segundoApellido: {_txtAuthSegundoApellido.Text.Trim()}");
                datos.AppendLine($"correoElectronico: {_txtAuthCorreo.Text.Trim()}");
                datos.AppendLine($"usuarioEncriptado: {usuarioCifrado}");
                datos.AppendLine($"contrasenaEncriptada: {contrasenaCifrada}");
                datos.AppendLine();
                datos.AppendLine("WS_AUTENTICACION2 / CambiarEstadoUsuario");
                datos.AppendLine($"identificacion: {_txtAuthIdentificacion.Text.Trim()}");
                datos.AppendLine($"estado: {estado}");

                _txtDatosUsuarioWcf.Text = datos.ToString();
            }
            catch (Exception ex)
            {
                _txtDatosUsuarioWcf.Text = "No se pudieron cifrar credenciales: " + ex.Message;
            }
        }

        private void ActualizarDatosFacturacionWcf()
        {
            string fechaCalculo = _txtFactFechaCalculo.Text.Trim();
            string fechaMaximaPago = _txtFactFechaMaximaPago.Text.Trim();
            bool fechasValidas = DateTime.TryParseExact(
                    fechaCalculo,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime calculo)
                && DateTime.TryParseExact(
                    fechaMaximaPago,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime maxPago)
                && maxPago >= calculo;

            _txtDatosFacturacionWcf.Text =
                "WS_PROVEEDOR3 / CalcularFacturacion" + Environment.NewLine +
                $"FechaCalculo: {fechaCalculo}" + Environment.NewLine +
                $"FechaMaximaPago: {fechaMaximaPago}" + Environment.NewLine +
                Environment.NewLine +
                (fechasValidas
                    ? "Validacion local: fechas validas para WCF."
                    : "Validacion local: revise formato o rango de fechas.");
        }

        private async Task ConsultarUsuariosMongoAsync()
        {
            _estado.Text = "Consultando usuarios en MongoDB...";
            var resultado = await _mongoUsuariosService.ConsultarUsuariosAsync();

            _gridUsuarios.DataSource = resultado.Usuarios
                .Select(u => new
                {
                    u.Identificacion,
                    u.Nombre,
                    u.PrimerApellido,
                    u.Correo,
                    u.Estado,
                    u.Tipo,
                    Usuario = UiTheme.ResumirIdentificador(u.UsuarioCifrado, 28)
                })
                .ToList();

            _estado.Text = resultado.Exitoso
                ? $"{resultado.Mensaje} Total: {resultado.Usuarios.Count}."
                : resultado.Mensaje;
        }

        private void SincronizarUsuarioSeleccionado()
        {
            if (_gridUsuarios.CurrentRow == null)
            {
                return;
            }

            _txtAuthIdentificacion.Text = _gridUsuarios.CurrentRow.Cells["Identificacion"]?.Value?.ToString() ?? "";
            _txtAuthNombre.Text = _gridUsuarios.CurrentRow.Cells["Nombre"]?.Value?.ToString() ?? "";
            _txtAuthPrimerApellido.Text = _gridUsuarios.CurrentRow.Cells["PrimerApellido"]?.Value?.ToString() ?? "";
            _txtAuthCorreo.Text = _gridUsuarios.CurrentRow.Cells["Correo"]?.Value?.ToString() ?? "";
            _cmbAuthEstado.Text = _gridUsuarios.CurrentRow.Cells["Estado"]?.Value?.ToString() ?? "activo";
            _cmbAuthTipo.Text = _gridUsuarios.CurrentRow.Cells["Tipo"]?.Value?.ToString() ?? "1";
            ActualizarDatosUsuarioWcf();
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

        private static string SoloDigitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        private void CopiarDatosWcf()
        {
            CopiarTexto(_txtDatosWcf, "Datos WCF copiados al portapapeles.");
        }

        private void CopiarTexto(TextBox origen, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(origen.Text))
            {
                _estado.Text = "No hay datos para copiar.";
                return;
            }

            Clipboard.SetText(origen.Text);
            _estado.Text = mensaje;
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
