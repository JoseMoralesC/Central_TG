using System.Drawing;
using System.Windows.Forms;

namespace SimuladorTelefonico.UI
{
    public class BitacoraForm : Form
    {
        public BitacoraForm(string contenido)
        {
            Text = "Bitacora local";
            UiTheme.ConfigurarVentana(this, new Size(780, 560), new Size(620, 420));

            Label titulo = new Label
            {
                Text = "Bitacora del simulador",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(28, 22),
                Size = new Size(520, 34)
            };

            Label subtitulo = UiTheme.CrearEtiqueta(
                "Registros locales generados por el simulador C#.",
                28,
                58,
                700,
                24,
                9.5f,
                FontStyle.Regular,
                UiTheme.TextoSecundario);

            TextBox txtContenido = new TextBox
            {
                Location = new Point(28, 98),
                Size = new Size(708, 350),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Text = contenido,
                BackColor = UiTheme.PanelDatos,
                ForeColor = UiTheme.Texto,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9)
            };

            Button btnCerrar = UiTheme.CrearBoton(
                "Cerrar",
                568,
                468,
                168,
                38,
                Color.FromArgb(52, 60, 72));
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(titulo);
            Controls.Add(subtitulo);
            Controls.Add(txtContenido);
            Controls.Add(btnCerrar);
        }
    }
}
