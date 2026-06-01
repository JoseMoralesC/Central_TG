using System;
using System.IO;

namespace SimuladorTelefonico.Services
{
    public class BitacoraService
    {
        private readonly string _rutaBitacora;

        public BitacoraService()
        {
            string carpetaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            if (!Directory.Exists(carpetaLogs))
            {
                Directory.CreateDirectory(carpetaLogs);
            }

            _rutaBitacora = Path.Combine(carpetaLogs, "bitacora_simulador.txt");
        }

        public void RegistrarEvento(string tipo, string contenido)
        {
            string linea =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {tipo}{Environment.NewLine}" +
                $"{contenido}{Environment.NewLine}" +
                $"--------------------------------------------------{Environment.NewLine}";

            File.AppendAllText(_rutaBitacora, linea);
        }
    }
}