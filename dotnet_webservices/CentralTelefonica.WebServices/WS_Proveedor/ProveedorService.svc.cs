using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

        public UltimaFacturacionResponse ObtenerUltimaFacturacion()
        {
            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["SqlServerProveedor"]?.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return CrearRespuestaErrorUltimaFacturacion(
                        "No se configuro la conexion SqlServerProveedor.");
                }

                const string sql = @"
SELECT TOP 1
    fecha_calculo,
    fecha_maxima_pago,
    COUNT(*) AS total_lineas,
    SUM(total_llamadas) AS total_llamadas,
    SUM(total_facturar) AS total_facturar,
    MAX(fecha_registro) AS fecha_registro
FROM dbo.facturacion_postpago
GROUP BY fecha_calculo, fecha_maxima_pago
ORDER BY fecha_calculo DESC, MAX(fecha_registro) DESC;";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    conexion.Open();

                    using (var reader = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            return new UltimaFacturacionResponse
                            {
                                Resultado = true,
                                Mensaje = "No existe facturacion previa.",
                                HayFacturacion = false
                            };
                        }

                        DateTime fechaCalculo = reader.GetDateTime(0);
                        DateTime fechaMaximaPago = reader.GetDateTime(1);
                        DateTime fechaRegistro = reader.GetDateTime(5);

                        return new UltimaFacturacionResponse
                        {
                            Resultado = true,
                            Mensaje = "Exitoso",
                            HayFacturacion = true,
                            FechaCalculo = fechaCalculo.ToString("yyyy-MM-dd"),
                            FechaMaximaPago = fechaMaximaPago.ToString("yyyy-MM-dd"),
                            TotalLineas = Convert.ToInt32(reader["total_lineas"]),
                            TotalLlamadas = Convert.ToInt32(reader["total_llamadas"]),
                            TotalFacturar = Convert.ToDecimal(reader["total_facturar"]),
                            FechaRegistro = fechaRegistro.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ObtenerUltimaFacturacion: " + ex);
                return CrearRespuestaErrorUltimaFacturacion();
            }
        }

        private static RespuestaServicio CrearRespuestaErrorActivacion(
            string detalle = null)
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje = string.IsNullOrWhiteSpace(detalle)
                    ? "Problemas al activar/desactivar la linea."
                    : detalle.Trim()
            };
        }

        private static RespuestaServicio CrearRespuestaErrorFacturacion()
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje =
                    "Problemas al realizar el calculo."
            };
        }

        private static UltimaFacturacionResponse CrearRespuestaErrorUltimaFacturacion(
            string detalle = null)
        {
            return new UltimaFacturacionResponse
            {
                Resultado = false,
                Mensaje = string.IsNullOrWhiteSpace(detalle)
                    ? "Problemas al consultar la ultima facturacion."
                    : detalle.Trim(),
                HayFacturacion = false
            };
        }
    }
}
