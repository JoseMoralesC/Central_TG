using SimuladorTelefonico.UI;

namespace SimuladorTelefonico
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new SeleccionTelefonoForm());
        }
    }
}