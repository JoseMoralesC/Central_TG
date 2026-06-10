using System.Drawing;
using System.Windows.Forms;

namespace SimuladorTelefonico.UI
{
    public class BitacoraForm : Form
    {
        public BitacoraForm(string contenido)
        {
            Text = "Bitacora local";
            UiTheme.ConfigurarVentana(this, new Size(900, 640), new Size(720, 500));
            Padding = new Padding(24);

            RoundedPanel contenedor = UiTheme.CrearPanelRedondeado(
                24,
                24,
                ClientSize.Width - 48,
                ClientSize.Height - 48,
                UiTheme.Superficie,
                20);
            contenedor.Anchor = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            Panel barraAcento = new Panel
            {
                BackColor = UiTheme.Primario,
                Location = new Point(24, 24),
                Size = new Size(contenedor.Width - 48, 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label etiqueta = UiTheme.CrearEtiqueta(
                "REGISTRO LOCAL",
                28,
                30,
                150,
                22,
                8.5f,
                FontStyle.Bold,
                UiTheme.Primario);

            Label titulo = new Label
            {
                Text = "Bitacora del simulador",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(28, 54),
                Size = new Size(540, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label subtitulo = UiTheme.CrearEtiqueta(
                "Registros locales generados por el simulador C#",
                28,
                91,
                520,
                24,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario);
            subtitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label estado = UiTheme.CrearEtiqueta(
                "Solo lectura",
                contenedor.Width - 178,
                58,
                118,
                30,
                9f,
                FontStyle.Bold,
                UiTheme.Exito,
                ContentAlignment.MiddleCenter);
            estado.BackColor = UiTheme.PanelDatos;
            estado.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            RoundedPanel panelContenido = UiTheme.CrearPanelRedondeado(
                28,
                128,
                contenedor.Width - 56,
                contenedor.Height - 214,
                UiTheme.PanelDatos,
                14);
            panelContenido.Anchor = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            RichTextBox txtContenido = new RichTextBox
            {
                Location = new Point(16, 14),
                Size = new Size(panelContenido.Width - 32, panelContenido.Height - 28),
                ReadOnly = true,
                Text = contenido,
                BackColor = UiTheme.PanelDatos,
                ForeColor = UiTheme.Texto,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10),
                WordWrap = false,
                ScrollBars = RichTextBoxScrollBars.Both,
                DetectUrls = false,
                ShortcutsEnabled = true,
                Anchor = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right
            };

            Button btnCerrar = UiTheme.CrearBoton(
                "Cerrar",
                contenedor.Width - 196,
                contenedor.Height - 62,
                168,
                38,
                Color.FromArgb(52, 60, 72));
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Click += (s, e) => Close();

            panelContenido.Controls.Add(txtContenido);

            contenedor.Controls.Add(barraAcento);
            contenedor.Controls.Add(etiqueta);
            contenedor.Controls.Add(titulo);
            contenedor.Controls.Add(subtitulo);
            contenedor.Controls.Add(estado);
            contenedor.Controls.Add(panelContenido);
            contenedor.Controls.Add(btnCerrar);

            Controls.Add(contenedor);
        }
    }
}
