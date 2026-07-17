using System;
using System.Diagnostics;
using WS_Proveedor.Infrastructure;
using WS_Proveedor.Models;
using WS_Proveedor.Services;
using WS_Proveedor.Validators;

namespace WS_Proveedor
{
    public class ProveedorService : IProveedorService
    {
        public RespuestaServicio ActivarDesactivarLinea(
            ActivarDesactivarLineaRequest solicitud)
        {
            try
            {
                if (!ActivarDesactivarLineaValidator.EsValida(solicitud))
                {
                    return CrearRespuestaError();
                }

                var tramaService = new TramaProveedorService();

                string tramaJson =
                    tramaService.ConstruirTramaProveedor5(solicitud);

                Debug.WriteLine(
                    "Trama PROVEEDOR5 generada: " + tramaJson);

                ProveedorClientOptions opciones =
                    ProveedorClientOptions.DesdeConfiguracion();

                var clienteProveedor =
                    new ProveedorTcpClient(opciones);

                string respuestaProveedor =
                    clienteProveedor.EnviarTrama(tramaJson);

                Debug.WriteLine(
                    "Respuesta de PROVEEDOR5: " +
                    respuestaProveedor);

                if (string.Equals(
                    respuestaProveedor,
                    "OK",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new RespuestaServicio
                    {
                        Resultado = true,
                        Mensaje = "Exitoso"
                    };
                }

                return CrearRespuestaError();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error en WS_PROVEEDOR2: " + ex);

                return CrearRespuestaError();
            }
        }

        private static RespuestaServicio CrearRespuestaError()
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje =
                    "Problemas al activar/desactivar la línea."
            };
        }
    }
}