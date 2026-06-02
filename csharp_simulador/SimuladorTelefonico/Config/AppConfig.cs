using System.Collections.Generic;
using System.Linq;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Config
{
    public static class AppConfig
    {
        public static string HostIdentificador { get; } = "127.0.0.1";

        public static int PuertoIdentificador { get; } = 5000;

        public static List<TelefonoVirtual> TelefonosVirtuales { get; } = new()
        {
            new TelefonoVirtual
            {
                Id = "TEL-001",
                Nombre = "Teléfono #1",
                Numero = "88888888",
                Maquina = "Máquina #1",
                IdentificadorDispositivo = "DISP-CSHARP-001",
                IdentificadorTarjeta = "SIM-CR-000000000000001",
                TipoServicio = "PREPAGO",
                TipoLlamada = "NACIONAL"
            },
            new TelefonoVirtual
            {
                Id = "TEL-002",
                Nombre = "Teléfono #2",
                Numero = "88889999",
                Maquina = "Máquina #1",
                IdentificadorDispositivo = "DISP-CSHARP-002",
                IdentificadorTarjeta = "SIM-CR-000000000000002",
                TipoServicio = "PREPAGO",
                TipoLlamada = "NACIONAL"
            },
            new TelefonoVirtual
            {
                Id = "TEL-003",
                Nombre = "Teléfono #3",
                Numero = "60001111",
                Maquina = "Máquina #2",
                IdentificadorDispositivo = "DISP-CSHARP-003",
                IdentificadorTarjeta = "SIM-CR-000000000000003",
                TipoServicio = "POSTPAGO",
                TipoLlamada = "NACIONAL"
            },
            new TelefonoVirtual
            {
                Id = "TEL-004",
                Nombre = "Teléfono #4",
                Numero = "60002222",
                Maquina = "Máquina #2",
                IdentificadorDispositivo = "DISP-CSHARP-004",
                IdentificadorTarjeta = "SIM-CR-000000000000004",
                TipoServicio = "POSTPAGO",
                TipoLlamada = "NACIONAL"
            }
        };

        public static TelefonoVirtual TelefonoActual { get; private set; } = TelefonosVirtuales.First();

        public static void SeleccionarTelefono(string id)
        {
            TelefonoActual = TelefonosVirtuales.FirstOrDefault(t => t.Id == id)
                ?? TelefonosVirtuales.First();
        }

        public static string NumeroOrigen => TelefonoActual.Numero;

        public static string IdentificadorDispositivo => TelefonoActual.IdentificadorDispositivo;

        public static string IdentificadorTarjeta => TelefonoActual.IdentificadorTarjeta;

        public static string TipoServicio => TelefonoActual.TipoServicio;

        public static string TipoLlamada => TelefonoActual.TipoLlamada;

        public static string Pais { get; } = "Costa Rica";

        public static string Provincia { get; } = "San Jose";

        public static double Latitud { get; } = 9.9281;

        public static double Longitud { get; } = -84.0907;

        public static decimal CostoPorMinuto { get; } = 10.00m;

        public static string Moneda { get; } = "CRC";
    }
}