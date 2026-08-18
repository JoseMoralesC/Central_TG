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
                EsIdentificadorTelefonoValido(solicitud.IdentificadorTelefono) &&
                EsIdentificadorTarjetaValido(solicitud.IdentificadorTarjeta) &&
                EsBase64(solicitud.IdentificacionCliente);
        }

        private static bool EsIdentificadorTelefonoValido(string valor)
        {
            return EsBase64(valor) ||
                TienePrefijoYDigitos(valor, "ENC_IMEI_", 15, 16);
        }

        private static bool EsIdentificadorTarjetaValido(string valor)
        {
            return EsBase64(valor) ||
                TienePrefijoYDigitos(valor, "ENC_SIM_", 19, 19);
        }

        private static bool TienePrefijoYDigitos(
            string valor,
            string prefijo,
            int minimoDigitos,
            int maximoDigitos)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            string normalizado = valor.Trim();

            if (!normalizado.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string digitos = normalizado.Substring(prefijo.Length);

            if (digitos.Length < minimoDigitos ||
                digitos.Length > maximoDigitos)
            {
                return false;
            }

            foreach (char caracter in digitos)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
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
