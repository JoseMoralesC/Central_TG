using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using WS_Proveedor.Contracts;
using WS_Proveedor.Models;

namespace WS_Proveedor.Services
{
    public class TramaProveedorService
    {
        public string ConstruirTramaProveedor5(
            ActivarDesactivarLineaRequest solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            var trama = new TramaProveedor5
            {
                TipoTransaccion = "PROVEEDOR5",
                Telefono = solicitud.NumeroTelefono?.Trim(),

                IdentificadorDispositivo =
                    solicitud.IdentificadorTelefono?.Trim(),

                IdentificadorTarjeta =
                    solicitud.IdentificadorTarjeta?.Trim(),

                TipoServicio =
                    solicitud.Tipo?.Trim().ToUpperInvariant(),

                IdentificacionDueno =
                    solicitud.IdentificacionCliente?.Trim(),

                Accion = ConvertirEstadoEnAccion(solicitud.Estado),

                FechaHora = DateTime.Now.ToString(
                    "yyyy-MM-ddTHH:mm:ss")
            };

            return SerializarJson(trama);
        }

        private static string ConvertirEstadoEnAccion(string estado)
        {
            string valor = estado?
                .Trim()
                .ToLowerInvariant();

            if (valor == "activo")
            {
                return "ACTIVAR";
            }

            if (valor == "disponible")
            {
                return "DESACTIVAR";
            }

            throw new ArgumentException(
                "El estado indicado no es válido.",
                nameof(estado));
        }

        private static string SerializarJson(
            TramaProveedor5 trama)
        {
            var serializador =
                new DataContractJsonSerializer(
                    typeof(TramaProveedor5));

            using (var memoria = new MemoryStream())
            {
                serializador.WriteObject(memoria, trama);

                return Encoding.UTF8.GetString(
                    memoria.ToArray());
            }
        }
    }
}