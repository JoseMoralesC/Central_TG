using SimuladorTelefonico.UI;
using System.IO;

namespace SimuladorTelefonico
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            RegistrarArranque("Inicio Main");
            ApplicationConfiguration.Initialize();
            RegistrarArranque("ApplicationConfiguration listo");

            SeleccionTelefonoForm form = new SeleccionTelefonoForm();
            RegistrarArranque("SeleccionTelefonoForm construido");

            Application.Run(form);
            RegistrarArranque("Application.Run finalizado");
        }

        private static void RegistrarArranque(string mensaje)
        {
            try
            {
                string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(carpeta);
                File.AppendAllText(
                    Path.Combine(carpeta, "startup_simulador.txt"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}{Environment.NewLine}");
            }
            catch
            {
                // El log de arranque no debe impedir abrir la aplicacion.
            }
        }
    }
}
