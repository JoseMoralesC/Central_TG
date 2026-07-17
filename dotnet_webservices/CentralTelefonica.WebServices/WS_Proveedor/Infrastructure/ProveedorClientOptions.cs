using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WS_Proveedor.Infrastructure
{
    public class ProveedorClientOptions
    {
        public string Host { get; private set; }

        public int Puerto { get; private set; }

        public int TimeoutMs { get; private set; }

        public bool ModoSimulado { get; private set; }

        public static ProveedorClientOptions DesdeConfiguracion()
        {
            string host = ConfigurationManager.AppSettings["ProveedorHost"];
            string puertoTexto =
                ConfigurationManager.AppSettings["ProveedorPort"];
            string timeoutTexto =
                ConfigurationManager.AppSettings["ProveedorTimeoutMs"];
            string modoSimuladoTexto =
                ConfigurationManager.AppSettings["ProveedorModoSimulado"];

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ConfigurationErrorsException(
                    "No se configuró ProveedorHost.");
            }

            if (!int.TryParse(puertoTexto, out int puerto) ||
                puerto <= 0 ||
                puerto > 65535)
            {
                throw new ConfigurationErrorsException(
                    "ProveedorPort no contiene un puerto válido.");
            }

            if (!int.TryParse(timeoutTexto, out int timeoutMs) ||
                timeoutMs <= 0)
            {
                throw new ConfigurationErrorsException(
                    "ProveedorTimeoutMs no contiene un valor válido.");
            }

            bool modoSimulado = false;

            if (!string.IsNullOrWhiteSpace(modoSimuladoTexto))
            {
                bool.TryParse(
                    modoSimuladoTexto,
                    out modoSimulado);
            }

            return new ProveedorClientOptions
            {
                Host = host.Trim(),
                Puerto = puerto,
                TimeoutMs = timeoutMs,
                ModoSimulado = modoSimulado
            };
        }
    }
}