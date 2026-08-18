using System;
using System.Collections.Generic;
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

        public ListadoLineasResponse ListarLineasDisponibles()
        {
            return ListarLineas("DISPONIBLE");
        }

        public ListadoLineasResponse ListarLineasActivas()
        {
            return ListarLineas("ACTIVO");
        }

        public RespuestaServicio RegistrarLinea(RegistrarLineaAdministrativaRequest solicitud)
        {
            try
            {
                if (solicitud == null ||
                    string.IsNullOrWhiteSpace(solicitud.NumeroTelefono) ||
                    string.IsNullOrWhiteSpace(solicitud.IdentificadorTelefono) ||
                    string.IsNullOrWhiteSpace(solicitud.IdentificadorTarjeta) ||
                    !TipoServicioValido(solicitud.TipoServicio))
                {
                    return CrearRespuestaError("Error al realizar el proceso");
                }

                string connectionString = ObtenerConnectionString();

                using (var conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();

                    using (var transaccion = conexion.BeginTransaction())
                    {
                        if (ExisteTelefono(conexion, transaccion, solicitud.NumeroTelefono.Trim()))
                        {
                            transaccion.Rollback();
                            return CrearRespuestaError("El numero ya existe.");
                        }

                        int clienteId = CrearClienteInventario(conexion, transaccion, solicitud.NumeroTelefono.Trim());
                        int servicioId = CrearServicioDisponible(conexion, transaccion, clienteId, solicitud);
                        AsegurarSaldo(conexion, transaccion, servicioId);
                        transaccion.Commit();
                    }
                }

                return new RespuestaServicio
                {
                    Resultado = true,
                    Mensaje = "Proceso finalizado de forma exitosa"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en RegistrarLinea: " + ex);
                return CrearRespuestaError("Error al realizar el proceso");
            }
        }

        public RespuestaServicio EliminarLineaDisponible(int servicioId)
        {
            try
            {
                if (servicioId <= 0)
                {
                    return CrearRespuestaError("Error al realizar el proceso");
                }

                string connectionString = ObtenerConnectionString();

                using (var conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();

                    using (var transaccion = conexion.BeginTransaction())
                    {
                        int clienteId = 0;

                        const string validar = @"
SELECT TOP 1 cliente_id
FROM dbo.servicios
WHERE servicio_id = @servicioId
  AND activo = 0
  AND UPPER(ISNULL(estado_linea, 'DISPONIBLE')) = 'DISPONIBLE';";

                        using (var comando = new SqlCommand(validar, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@servicioId", servicioId);
                            object valor = comando.ExecuteScalar();

                            if (valor == null || valor == DBNull.Value)
                            {
                                transaccion.Rollback();
                                return CrearRespuestaError("Error al realizar el proceso");
                            }

                            clienteId = Convert.ToInt32(valor);
                        }

                        EjecutarSinResultado(conexion, transaccion,
                            "DELETE FROM dbo.saldos WHERE servicio_id = @servicioId",
                            servicioId);

                        EjecutarSinResultado(conexion, transaccion,
                            "DELETE FROM dbo.servicios WHERE servicio_id = @servicioId",
                            servicioId);

                        EliminarClienteInventarioSiNoTieneLineas(conexion, transaccion, clienteId);
                        transaccion.Commit();
                    }
                }

                return new RespuestaServicio
                {
                    Resultado = true,
                    Mensaje = "Proceso finalizado de forma exitosa"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en EliminarLineaDisponible: " + ex);
                return CrearRespuestaError("Error al realizar el proceso");
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

        private static ListadoLineasResponse ListarLineas(string estado)
        {
            try
            {
                var lineas = new List<LineaAdministrativaDto>();
                string connectionString = ObtenerConnectionString();

                const string sql = @"
SELECT
    s.servicio_id,
    s.numero_telefono,
    ISNULL(s.identificador_telefono_cifrado, '') AS identificador_telefono_cifrado,
    ISNULL(s.identificador_tarjeta_cifrado, '') AS identificador_tarjeta_cifrado,
    ISNULL(s.tipo_servicio, '') AS tipo_servicio,
    ISNULL(s.identificacion_dueno_cifrada, '') AS identificacion_dueno_cifrada,
    ISNULL(c.nombre, '') AS nombre_cliente,
    ISNULL(s.estado_linea, CASE WHEN s.activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END) AS estado_linea,
    s.activo,
    ISNULL(s.proveedor_codigo, 'KOLBI') AS proveedor_codigo
FROM dbo.servicios s
LEFT JOIN dbo.clientes c ON c.cliente_id = s.cliente_id
WHERE UPPER(ISNULL(s.estado_linea, CASE WHEN s.activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END)) = @estado
ORDER BY s.numero_telefono;";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@estado", estado);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lineas.Add(new LineaAdministrativaDto
                            {
                                ServicioId = Convert.ToInt32(reader["servicio_id"]),
                                NumeroTelefono = Convert.ToString(reader["numero_telefono"]),
                                IdentificadorTelefono = Convert.ToString(reader["identificador_telefono_cifrado"]),
                                IdentificadorTarjeta = Convert.ToString(reader["identificador_tarjeta_cifrado"]),
                                TipoServicio = Convert.ToString(reader["tipo_servicio"]),
                                IdentificacionCliente = Convert.ToString(reader["identificacion_dueno_cifrada"]),
                                NombreCliente = Convert.ToString(reader["nombre_cliente"]),
                                EstadoLinea = Convert.ToString(reader["estado_linea"]),
                                Activo = Convert.ToBoolean(reader["activo"]),
                                ProveedorCodigo = Convert.ToString(reader["proveedor_codigo"])
                            });
                        }
                    }
                }

                return new ListadoLineasResponse
                {
                    Resultado = true,
                    Mensaje = lineas.Count == 0 ? "No hay lineas para mostrar." : "Exitoso",
                    Lineas = lineas
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ListarLineas: " + ex);
                return new ListadoLineasResponse
                {
                    Resultado = false,
                    Mensaje = "Error al realizar el proceso",
                    Lineas = new List<LineaAdministrativaDto>()
                };
            }
        }

        private static string ObtenerConnectionString()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["SqlServerProveedor"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException("No se configuro la conexion SqlServerProveedor.");
            }

            return connectionString;
        }

        private static bool TipoServicioValido(string tipoServicio)
        {
            string valor = tipoServicio?.Trim().ToUpperInvariant();
            return valor == "PREPAGO" || valor == "POSTPAGO";
        }

        private static bool ExisteTelefono(SqlConnection conexion, SqlTransaction transaccion, string numeroTelefono)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.servicios WHERE numero_telefono = @numeroTelefono";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@numeroTelefono", numeroTelefono);
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        private static int CrearClienteInventario(SqlConnection conexion, SqlTransaction transaccion, string numeroTelefono)
        {
            const string sql = @"
INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
VALUES (@nombre, @identificacion, @correo, 1);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@nombre", "Inventario " + numeroTelefono);
                comando.Parameters.AddWithValue("@identificacion", "INV-" + numeroTelefono);
                comando.Parameters.AddWithValue("@correo", numeroTelefono + "@inventario.central.test");
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        private static int CrearServicioDisponible(
            SqlConnection conexion,
            SqlTransaction transaccion,
            int clienteId,
            RegistrarLineaAdministrativaRequest solicitud)
        {
            const string sql = @"
INSERT INTO dbo.servicios
    (cliente_id, numero_telefono, tipo_servicio, proveedor_codigo, activo,
     estado_linea, identificador_telefono_cifrado, identificador_tarjeta_cifrado)
VALUES
    (@clienteId, @numeroTelefono, @tipoServicio, 'KOLBI', 0,
     'DISPONIBLE', @identificadorTelefono, @identificadorTarjeta);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@clienteId", clienteId);
                comando.Parameters.AddWithValue("@numeroTelefono", solicitud.NumeroTelefono.Trim());
                comando.Parameters.AddWithValue("@tipoServicio", solicitud.TipoServicio.Trim().ToUpperInvariant());
                comando.Parameters.AddWithValue("@identificadorTelefono", ProveedorCryptoService.Encriptar(solicitud.IdentificadorTelefono));
                comando.Parameters.AddWithValue("@identificadorTarjeta", ProveedorCryptoService.Encriptar(solicitud.IdentificadorTarjeta));
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        private static void AsegurarSaldo(SqlConnection conexion, SqlTransaction transaccion, int servicioId)
        {
            const string sql = "INSERT INTO dbo.saldos (servicio_id, saldo_disponible) VALUES (@servicioId, 0)";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@servicioId", servicioId);
                comando.ExecuteNonQuery();
            }
        }

        private static void EjecutarSinResultado(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string sql,
            int servicioId)
        {
            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@servicioId", servicioId);
                comando.ExecuteNonQuery();
            }
        }

        private static void EliminarClienteInventarioSiNoTieneLineas(
            SqlConnection conexion,
            SqlTransaction transaccion,
            int clienteId)
        {
            const string sql = @"
DELETE c
FROM dbo.clientes c
WHERE c.cliente_id = @clienteId
  AND c.identificacion LIKE 'INV-%'
  AND NOT EXISTS (
      SELECT 1 FROM dbo.servicios s WHERE s.cliente_id = c.cliente_id
  );";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@clienteId", clienteId);
                comando.ExecuteNonQuery();
            }
        }

        private static RespuestaServicio CrearRespuestaError(string mensaje)
        {
            return new RespuestaServicio
            {
                Resultado = false,
                Mensaje = mensaje
            };
        }
    }
}
