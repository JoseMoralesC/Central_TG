using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            return true;
        }
    }
}