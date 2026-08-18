using System;
using System.Configuration;
using WS_ProveedorCliente.Models;
using WS_ProveedorCliente.Services;

namespace WS_ProveedorCliente
{
    public class ProveedorClienteService : IProveedorClienteService
    {
        private readonly LineaClienteService _lineaClienteService;

        public ProveedorClienteService()
        {
            string connectionString =
                ConfigurationManager.AppSettings["SqlServerConnection"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se configuro SqlServerConnection.");
            }

            int timeoutSegundos = 10;
            string timeoutTexto = ConfigurationManager.AppSettings["SqlServerTimeoutSec"];
            if (!int.TryParse(timeoutTexto, out timeoutSegundos) || timeoutSegundos <= 0)
            {
                timeoutSegundos = 10;
            }

            _lineaClienteService =
                new LineaClienteService(connectionString, timeoutSegundos);
        }

        public ConsultarLineasClienteResponse ConsultarLineasCliente(
            string identificacion)
        {
            return _lineaClienteService.Consultar(identificacion);
        }

        public RecargarSaldoResponse RecargarSaldo(string numeroTelefono, decimal monto)
        {
            return _lineaClienteService.Recargar(numeroTelefono, monto);
        }

        public PagarFacturaResponse PagarFactura(
            string numeroTelefono,
            decimal monto,
            string identificacion)
        {
            return _lineaClienteService.PagarFactura(
                numeroTelefono,
                monto,
                identificacion);
        }
    }
}