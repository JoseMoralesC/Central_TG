using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SimuladorTelefonico.UI
{
    internal static class UiTheme
    {
        public static readonly Color Fondo = Color.FromArgb(10, 13, 17);
        public static readonly Color Superficie = Color.FromArgb(18, 22, 28);
        public static readonly Color SuperficieElevada = Color.FromArgb(25, 30, 38);
        public static readonly Color PanelDatos = Color.FromArgb(13, 17, 23);
        public static readonly Color Borde = Color.FromArgb(42, 50, 62);
        public static readonly Color Texto = Color.FromArgb(242, 246, 250);
        public static readonly Color TextoSecundario = Color.FromArgb(174, 184, 197);
        public static readonly Color Primario = Color.FromArgb(35, 154, 224);
        public static readonly Color Exito = Color.FromArgb(74, 210, 132);
        public static readonly Color Advertencia = Color.FromArgb(255, 190, 86);
        public static readonly Color Error = Color.FromArgb(232, 88, 88);
        public static readonly Color Postpago = Color.FromArgb(134, 211, 140);

        public static readonly Color Activo = Color.FromArgb(35, 154, 224);

        public static void ConfigurarVentana(Form form, Size size, Size? minimumSize = null)
        {
            form.Size = size;
            form.MinimumSize = minimumSize ?? size;
            form.StartPosition = FormStartPosition.CenterParent;
            form.BackColor = Fondo;
            form.Font = new Font("Segoe UI", 9);
            form.MaximizeBox = false;
        }

        public static Label CrearEtiqueta(
            string texto,
            int x,
            int y,
            int ancho,
            int alto,
            float tamano,
            FontStyle estilo,
            Color color,
            ContentAlignment alineacion = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                Font = new Font("Segoe UI", tamano, estilo),
                ForeColor = color,
                BackColor = Color.Transparent,
                TextAlign = alineacion,
                AutoEllipsis = true
            };
        }

        public static Button CrearBoton(
            string texto,
            int x,
            int y,
            int ancho,
            int alto,
            Color color)
        {
            RoundedButton boton = new RoundedButton
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                BorderRadius = 14
            };

            return boton;
        }

        public static RoundedPanel CrearPanelRedondeado(
            int x,
            int y,
            int ancho,
            int alto,
            Color color,
            int radio = 18)
        {
            return new RoundedPanel
            {
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BackColor = color,
                BorderRadius = radio,
                BorderColor = Borde,
                BorderThickness = 1
            };
        }

        public static string FormatearSaldo(string tipoServicio, decimal saldo)
        {
            if (tipoServicio.Equals("POSTPAGO", StringComparison.OrdinalIgnoreCase)
                && saldo < 0)
            {
                return "Postpago";
            }

            return $"{saldo.ToString("N2", CultureInfo.CurrentCulture)} CRC";
        }

        public static string ResumirIdentificador(string valor, int visibles = 10)
        {
            if (string.IsNullOrWhiteSpace(valor) || valor.Length <= visibles)
            {
                return valor;
            }

            return $"{valor[..visibles]}...";
        }

        public static string AgruparIdentificador(string valor, int grupo = 4)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return "No disponible";
            }

            string limpio = valor.Replace(" ", string.Empty);
            if (limpio.Length <= grupo)
            {
                return limpio;
            }

            string[] grupos = Enumerable
                .Range(0, (int)Math.Ceiling(limpio.Length / (double)grupo))
                .Select(i =>
                {
                    int inicio = i * grupo;
                    int longitud = Math.Min(grupo, limpio.Length - inicio);
                    return limpio.Substring(inicio, longitud);
                })
                .ToArray();

            return string.Join(" ", grupos);
        }

        private static Color Aclarar(Color color)
        {
            return Color.FromArgb(
                Math.Min(255, color.R + 14),
                Math.Min(255, color.G + 14),
                Math.Min(255, color.B + 14));
        }

        private static Color Oscurecer(Color color)
        {
            return Color.FromArgb(
                Math.Max(0, color.R - 14),
                Math.Max(0, color.G - 14),
                Math.Max(0, color.B - 14));
        }
    }

    internal class RoundedPanel : Panel
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius { get; set; } = 18;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = UiTheme.Borde;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderThickness { get; set; } = 1;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using GraphicsPath path = CrearRutaRedondeada(rect, BorderRadius);
            using SolidBrush brush = new SolidBrush(BackColor);
            using Pen pen = new Pen(BorderColor, BorderThickness);

            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(pen, path);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using GraphicsPath path = CrearRutaRedondeada(ClientRectangle, BorderRadius);
            Region = new Region(path);
        }

        private static GraphicsPath CrearRutaRedondeada(Rectangle rect, int radio)
        {
            int diametro = Math.Max(1, radio * 2);
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.Left, rect.Top, diametro, diametro, 180, 90);
            path.AddArc(rect.Right - diametro, rect.Top, diametro, diametro, 270, 90);
            path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diametro, diametro, diametro, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    internal class RoundedButton : Button
    {
        private bool _hover;
        private bool _pressed;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius { get; set; } = 14;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            DoubleBuffered = true;
            ResizeRedraw = true;
            UseVisualStyleBackColor = false;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            Rectangle rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using GraphicsPath path = RoundedPanelPath(rect, BorderRadius);
            Region = new Region(path);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            _pressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _pressed = true;
            Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color color = BackColor;
            if (_pressed)
            {
                color = ControlPaint.Dark(BackColor, 0.12f);
            }
            else if (_hover)
            {
                color = ControlPaint.Light(BackColor, 0.12f);
            }

            Rectangle rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using GraphicsPath path = RoundedPanelPath(rect, BorderRadius);
            using SolidBrush brush = new SolidBrush(color);
            using SolidBrush textBrush = new SolidBrush(ForeColor);

            pevent.Graphics.FillPath(brush, path);

            TextRenderer.DrawText(
                pevent.Graphics,
                Text,
                Font,
                rect,
                ForeColor,
                TextFormatFlags.HorizontalCenter
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis);
        }

        private static GraphicsPath RoundedPanelPath(Rectangle rect, int radio)
        {
            int diametro = Math.Max(1, radio * 2);
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.Left, rect.Top, diametro, diametro, 180, 90);
            path.AddArc(rect.Right - diametro, rect.Top, diametro, diametro, 270, 90);
            path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diametro, diametro, diametro, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
