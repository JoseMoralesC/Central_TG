using System;
using WS_Proveedor.Models;

namespace WS_Proveedor.Validators
{
    public static class ActivarDesactivarLineaValidator
    {
        public static bool EsValida(ActivarDesactivarLineaRequest solicitud)
        {
            if (solicitud == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(solicitud.NumeroTelefono) ||
                string.IsNullOrWhiteSpace(solicitud.IdentificadorTelefono) ||
                string.IsNullOrWhiteSpace(solicitud.IdentificadorTarjeta) ||
                string.IsNullOrWhiteSpace(solicitud.Tipo) ||
                string.IsNullOrWhiteSpace(solicitud.IdentificacionCliente) ||
                string.IsNullOrWhiteSpace(solicitud.Estado))
            {
                return false;
            }

            string tipoNormalizado = solicitud.Tipo
                .Trim()
                .ToLowerInvariant();

            if (tipoNormalizado != "prepago" &&
                tipoNormalizado != "postpago")
            {
                return false;
            }

            string estadoNormalizado = solicitud.Estado
                .Trim()
                .ToLowerInvariant();

            if (estadoNormalizado != "activo" &&
                estadoNormalizado != "disponible")
            {
                return false;
            }

            return EsBase64(solicitud.NumeroTelefono) &&
                EsBase64(solicitud.IdentificadorTelefono) &&
                EsBase64(solicitud.IdentificadorTarjeta) &&
                EsBase64(solicitud.IdentificacionCliente);
        }

        private static bool EsBase64(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(valor.Trim());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
