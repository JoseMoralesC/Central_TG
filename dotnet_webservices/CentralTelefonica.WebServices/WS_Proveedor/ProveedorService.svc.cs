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
                    return CrearRespuestaErrorActivacion();
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

                return CrearRespuestaErrorActivacion(respuestaProveedor);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error en WS_PROVEEDOR2: " + ex);

                return CrearRespuestaErrorActivacion();
            }
        }

        public RespuestaServicio CalcularFacturacion(
            CalcularFacturacionRequest solicitud)
        {
            try
            {
                if (!CalcularFacturacionValidator.EsValida(solicitud))
                {
                    return CrearRespuestaErrorFacturacion();
                }

                string tramaJson =
                    TramaProveedor6Service.ConstruirTrama(solicitud);

                Debug.WriteLine(
                    "Trama PROVEEDOR6 generada: " + tramaJson);

                ProveedorClientOptions opciones =
                    ProveedorClientOptions.DesdeConfiguracion();

                var clienteProveedor =
                    new ProveedorTcpClient(opciones);

                string respuestaProveedor =
                    clienteProveedor.EnviarTrama(tramaJson);

                Debug.WriteLine(
                    "Respuesta de PROVEEDOR6: " +
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

                return CrearRespuestaErrorFacturacion();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Error en WS_PROVEEDOR3: " + ex);

                return CrearRespuestaErrorFacturacion();
            }
        }

        private static RespuestaServicio CrearRespuestaErrorActivacion(
            string detalle = null)
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje = string.IsNullOrWhiteSpace(detalle)
                    ? "Problemas al activar/desactivar la línea."
                    : detalle.Trim()
            };
        }

        private static RespuestaServicio CrearRespuestaErrorFacturacion()
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje =
                    "Problemas al realizar el cálculo."
            };
        }
    }
}
