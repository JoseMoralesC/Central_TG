using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimuladorTelefonico.Models;
using SimuladorTelefonico.Services;

namespace SimuladorTelefonico.Config
{
    public static class AppConfig
    {
        public static string HostIdentificador { get; } =
            ObtenerConfig("IDENTIFICADOR_HOST", "127.0.0.1");

        public static int PuertoIdentificador { get; } =
            ObtenerEntero("IDENTIFICADOR_PORT", 5000);

        public static int TimeoutConexionMs { get; } =
            ObtenerEntero("CSHARP_SOCKET_CONNECT_TIMEOUT_MS", 5000);

        public static int TimeoutLecturaMs { get; } =
            ObtenerEntero("CSHARP_SOCKET_READ_TIMEOUT_MS", 8000);

        public static int BufferRespuestaBytes { get; } =
            ObtenerEntero("CSHARP_SOCKET_BUFFER_BYTES", 8192);

        public static string ModoTrama { get; } =
            ObtenerConfig("CSHARP_TRAMA_MODO", "JSON");

        public static string SocketEncoding { get; } =
            ObtenerConfig("SOCKET_ENCODING", "UTF-8");

        public static string AesKey { get; } = ObtenerAesKey();

        public static string AesIv { get; } =
            ObtenerConfig("AES_IV", "VectorInicio1234");

        public static bool CifradoSensiblesActivo { get; } =
            ObtenerBooleano("CSHARP_AES_ACTIVO", true);

        public static List<TelefonoVirtual> TelefonosVirtuales { get; private set; } =
            TelefonoCatalogoService.CargarTelefonos();

        public static string FuenteDatosTelefonos =>
            TelefonoCatalogoService.FuenteDatos;

        public static TelefonoVirtual TelefonoActual { get; private set; } =
            TelefonosVirtuales.First();

        public static void SeleccionarTelefono(string id)
        {
            TelefonoActual = TelefonosVirtuales.FirstOrDefault(t => t.Id == id)
                ?? TelefonosVirtuales.First();
        }

        public static void RecargarCatalogoTelefonos()
        {
            List<TelefonoVirtual> telefonos = TelefonoCatalogoService.CargarTelefonos(
                consultarBackend: true);

            ActualizarCatalogoTelefonos(telefonos);
        }

        public static void ActualizarCatalogoTelefonos(List<TelefonoVirtual> telefonos)
        {
            if (telefonos.Count == 0)
            {
                return;
            }

            string idActual = TelefonoActual.Id;
            TelefonosVirtuales = telefonos;
            TelefonoActual = TelefonosVirtuales.FirstOrDefault(t => t.Id == idActual)
                ?? TelefonosVirtuales.First();
        }

        public static void ActualizarSaldoTelefonoActual(decimal saldoDisponible)
        {
            TelefonoActual.SaldoDisponible = saldoDisponible;
        }

        public static string NumeroOrigen => TelefonoActual.Numero;

        public static string IdentificadorTelefono =>
            TelefonoActual.IdentificadorDispositivo;

        public static string IdentificadorDispositivo =>
            TelefonoActual.IdentificadorDispositivo;

        public static string IdentificadorTarjeta =>
            TelefonoActual.IdentificadorTarjeta;

        public static string TipoServicio => TelefonoActual.TipoServicio;

        public static string TipoLlamada => TelefonoActual.TipoLlamada;

        public static string Pais { get; } = "Costa Rica";

        public static string Provincia { get; } = "San Jose";

        public static double Latitud { get; } = 9.9281;

        public static double Longitud { get; } = -84.0907;

        public static decimal CostoPorMinuto { get; } = 10.00m;

        public static string Moneda { get; } = "CRC";

        private static string ObtenerConfig(string llave, string valorPorDefecto)
        {
            string? valor = Environment.GetEnvironmentVariable(llave);

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }

            valor = ObtenerDesdeArchivoEnv(llave);

            return string.IsNullOrWhiteSpace(valor)
                ? valorPorDefecto
                : valor.Trim();
        }

        private static int ObtenerEntero(string llave, int valorPorDefecto)
        {
            string valor = ObtenerConfig(llave, valorPorDefecto.ToString());

            return int.TryParse(valor, out int numero)
                ? numero
                : valorPorDefecto;
        }

        private static bool ObtenerBooleano(string llave, bool valorPorDefecto)
        {
            string valor = ObtenerConfig(llave, valorPorDefecto ? "true" : "false");

            if (bool.TryParse(valor, out bool resultado))
            {
                return resultado;
            }

            return valor.Equals("1", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("SI", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("YES", StringComparison.OrdinalIgnoreCase);
        }

        private static string ObtenerAesKey()
        {
            string aesKey = ObtenerConfig("AES_KEY", string.Empty);

            if (TieneLongitudAesValida(aesKey))
            {
                return aesKey;
            }

            string aesSecretKey = ObtenerConfig("AES_SECRET_KEY", string.Empty);

            if (TieneLongitudAesValida(aesSecretKey))
            {
                return aesSecretKey;
            }

            return "ClaveSecreta1234";
        }

        private static bool TieneLongitudAesValida(string valor)
        {
            int longitud = System.Text.Encoding.UTF8.GetByteCount(valor);

            return longitud == 16 || longitud == 24 || longitud == 32;
        }

        private static string? ObtenerDesdeArchivoEnv(string llave)
        {
            DirectoryInfo? directorio = new(AppDomain.CurrentDomain.BaseDirectory);

            while (directorio != null)
            {
                string rutaEnv = Path.Combine(directorio.FullName, ".env");

                if (File.Exists(rutaEnv))
                {
                    foreach (string linea in File.ReadAllLines(rutaEnv))
                    {
                        string texto = linea.Trim();

                        if (texto.Length == 0 || texto.StartsWith("#"))
                        {
                            continue;
                        }

                        int separador = texto.IndexOf('=');

                        if (separador <= 0)
                        {
                            continue;
                        }

                        string nombre = texto[..separador].Trim();

                        if (!nombre.Equals(llave, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        return texto[(separador + 1)..].Trim();
                    }
                }

                directorio = directorio.Parent;
            }

            return null;
        }
    }
}
