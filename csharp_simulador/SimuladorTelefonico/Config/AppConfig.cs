namespace SimuladorTelefonico.Config
{
    public static class AppConfig
    {
        public static string HostIdentificador { get; } = "127.0.0.1";

        public static int PuertoIdentificador { get; } = 5000;

        public static string NumeroOrigen { get; } = "88888888";

        public static string IdentificadorDispositivo { get; } = "DISP-CSharp-001";

        public static string IdentificadorTarjeta { get; } = "SIM-CR-001";

        public static string TipoServicio { get; } = "PREPAGO";

        public static string TipoLlamada { get; } = "NACIONAL";

        public static string Pais { get; } = "Costa Rica";

        public static string Provincia { get; } = "San Jose";

        public static double Latitud { get; } = 9.9281;

        public static double Longitud { get; } = -84.0907;

        public static decimal CostoPorMinuto { get; } = 10.00m;

        public static string Moneda { get; } = "CRC";
    }
}