using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WS_Proveedor.Infrastructure;
using WS_Proveedor.Models;
using WS_Proveedor.Services;
using WS_Proveedor.Validators;

namespace WS_Proveedor
{
    public class ProveedorService : IProveedorService
    {
        public FacturacionConsultaResponse ConsultarFacturacion(
            CalcularFacturacionRequest solicitud)
        {
            try
            {
                if (!CalcularFacturacionValidator.EsValida(solicitud) ||
                    string.IsNullOrWhiteSpace(solicitud.NumeroTelefono))
                {
                    return CrearRespuestaErrorConsultaFacturacion(
                        "Ingrese linea postpago y fechas validas para consultar.");
                }

                string connectionString = ObtenerConnectionString();

                const string sql = @"
SELECT TOP 1
    s.numero_telefono,
    COUNT(lp.llamada_id) AS total_llamadas,
    ISNULL(SUM(lp.costo), 0.00) AS total_facturar
FROM dbo.servicios s
LEFT JOIN dbo.llamadas_proveedor lp
    ON lp.servicio_id = s.servicio_id
    AND lp.fecha_llamada >= @fechaCalculo
    AND lp.fecha_llamada <= @fechaMaximaPago
    AND COALESCE(lp.estado, 'FINALIZADA') = 'FINALIZADA'
WHERE UPPER(ISNULL(s.tipo_servicio, '')) = 'POSTPAGO'
  AND s.activo = 1
  AND UPPER(ISNULL(s.estado_linea, 'ACTIVO')) = 'ACTIVO'
  AND (
      s.numero_telefono = @numeroTelefono
      OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) =
         RIGHT(REPLACE(@numeroTelefono, '+', ''), 8)
  )
GROUP BY s.servicio_id, s.numero_telefono
ORDER BY s.servicio_id DESC;";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaCalculo", solicitud.FechaCalculo);
                    comando.Parameters.AddWithValue("@fechaMaximaPago", solicitud.FechaMaximaPago);
                    comando.Parameters.AddWithValue("@numeroTelefono", solicitud.NumeroTelefono.Trim());
                    conexion.Open();

                    using (var reader = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            return CrearRespuestaErrorConsultaFacturacion(
                                "No se encontro una linea postpago activa para consultar.");
                        }

                        int totalLlamadas = Convert.ToInt32(reader["total_llamadas"]);
                        decimal totalFacturar = Convert.ToDecimal(reader["total_facturar"]);
                        string numeroTelefono = Convert.ToString(reader["numero_telefono"]);

                        return new FacturacionConsultaResponse
                        {
                            Resultado = true,
                            Mensaje = "Consulta realizada. Puede generar la factura con este resultado.",
                            NumeroTelefono = numeroTelefono,
                            FechaCalculo = solicitud.FechaCalculo,
                            FechaMaximaPago = solicitud.FechaMaximaPago,
                            TotalLlamadas = totalLlamadas,
                            TotalFacturar = totalFacturar
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ConsultarFacturacion: " + ex);
                return CrearRespuestaErrorConsultaFacturacion();
            }
        }

        public RespuestaServicio ActivarDesactivarLinea(
            ActivarDesactivarLineaRequest solicitud)
        {
            try
            {
                if (!ActivarDesactivarLineaValidator.EsValida(solicitud))
                {
                    return CrearRespuestaErrorActivacion();
                }

                if (EsActivacion(solicitud.Estado) &&
                    NumeroTieneLineaActiva(solicitud.NumeroTelefono))
                {
                    return CrearRespuestaErrorActivacion(
                        "El numero ya se encuentra asignado a un cliente.");
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
                        Mensaje = "Proceso finalizado de forma exitosa"
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
                    string mensaje = CrearMensajeFacturacionExitosa(solicitud);

                    return new RespuestaServicio
                    {
                        Resultado = true,
                            Mensaje = "Factura generada. " + mensaje
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

        private static string CrearMensajeFacturacionExitosa(CalcularFacturacionRequest solicitud)
        {
            if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.NumeroTelefono))
            {
                return "Facturacion calculada correctamente.";
            }

            try
            {
                string connectionString = ObtenerConnectionString();

                const string sql = @"
SELECT TOP 1
    s.numero_telefono,
    f.total_llamadas,
    f.total_facturar
FROM dbo.facturacion_postpago f
JOIN dbo.servicios s ON s.servicio_id = f.servicio_id
WHERE f.fecha_calculo = @fechaCalculo
  AND (
      s.numero_telefono = @numeroTelefono
      OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) =
         RIGHT(REPLACE(@numeroTelefono, '+', ''), 8)
  )
ORDER BY f.facturacion_id DESC;";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaCalculo", solicitud.FechaCalculo);
                    comando.Parameters.AddWithValue("@numeroTelefono", solicitud.NumeroTelefono.Trim());
                    conexion.Open();

                    using (var reader = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            return "No se encontro facturacion para la linea seleccionada.";
                        }

                        return "Factura consultada para " +
                            Convert.ToString(reader["numero_telefono"]) +
                            " | Llamadas: " + Convert.ToInt32(reader["total_llamadas"]) +
                            " | Total: " + Convert.ToDecimal(reader["total_facturar"]).ToString("0.00") +
                            " CRC";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error consultando resumen de facturacion individual: " + ex);
                return "Facturacion calculada correctamente.";
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
    s.numero_telefono,
    f.fecha_calculo,
    f.fecha_maxima_pago,
    f.total_llamadas,
    f.total_facturar,
    f.fecha_registro
FROM dbo.facturacion_postpago f
JOIN dbo.servicios s ON s.servicio_id = f.servicio_id
ORDER BY f.fecha_registro DESC, f.facturacion_id DESC;";

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
                                Mensaje = "No existe calculo previo.",
                                HayFacturacion = false
                            };
                        }

                        DateTime fechaCalculo = reader.GetDateTime(1);
                        DateTime fechaMaximaPago = reader.GetDateTime(2);
                        DateTime fechaRegistro = reader.GetDateTime(5);

                        return new UltimaFacturacionResponse
                        {
                            Resultado = true,
                            Mensaje = "Exitoso",
                            HayFacturacion = true,
                            FechaCalculo = fechaCalculo.ToString("yyyy-MM-dd"),
                            FechaMaximaPago = fechaMaximaPago.ToString("yyyy-MM-dd"),
                            NumeroTelefono = Convert.ToString(reader["numero_telefono"]),
                            TotalLineas = 1,
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

        public RespuestaServicio SolicitarLineaCliente(SolicitarLineaClienteRequest solicitud)
        {
            try
            {
                if (solicitud == null ||
                    solicitud.ServicioId <= 0 ||
                    string.IsNullOrWhiteSpace(solicitud.NumeroTelefono) ||
                    string.IsNullOrWhiteSpace(solicitud.TipoServicio) ||
                    string.IsNullOrWhiteSpace(solicitud.IdentificacionCliente) ||
                    string.IsNullOrWhiteSpace(solicitud.NombreCliente) ||
                    !TipoServicioValido(solicitud.TipoServicio))
                {
                    return CrearRespuestaError("Debe seleccionar una linea disponible.");
                }

                string connectionString = ObtenerConnectionString();

                using (var conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();

                    if (!LineaDisponibleCoincide(conexion, solicitud))
                    {
                        return CrearRespuestaError("La linea seleccionada ya no esta disponible.");
                    }

                    if (ExisteSolicitudPendiente(conexion, solicitud.ServicioId, solicitud.IdentificacionCliente))
                    {
                        return CrearRespuestaError("Ya existe una solicitud pendiente para esta linea.");
                    }

                    const string sql = @"
INSERT INTO dbo.solicitudes_linea
    (servicio_id, numero_telefono, tipo_servicio, identificacion_cliente,
     nombre_cliente, estado)
VALUES
    (@servicioId, @numeroTelefono, @tipoServicio, @identificacionCliente,
     @nombreCliente, 'PENDIENTE');";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@servicioId", solicitud.ServicioId);
                        comando.Parameters.AddWithValue("@numeroTelefono", solicitud.NumeroTelefono.Trim());
                        comando.Parameters.AddWithValue("@tipoServicio", solicitud.TipoServicio.Trim().ToUpperInvariant());
                        comando.Parameters.AddWithValue("@identificacionCliente", solicitud.IdentificacionCliente.Trim());
                        comando.Parameters.AddWithValue("@nombreCliente", solicitud.NombreCliente.Trim());
                        comando.ExecuteNonQuery();
                    }
                }

                return new RespuestaServicio
                {
                    Resultado = true,
                    Mensaje = "Solicitud registrada. Un administrador revisara la asignacion."
                };
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Error SQL en SolicitarLineaCliente: " + ex);
                return CrearRespuestaError("No fue posible registrar la solicitud.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en SolicitarLineaCliente: " + ex);
                return CrearRespuestaError("No fue posible registrar la solicitud.");
            }
        }

        public ListadoLineasResponse ListarLineasActivas()
        {
            return ListarLineas("ACTIVO");
        }

        public RespuestaServicio ActualizarCorreoCliente(ActualizarCorreoClienteRequest solicitud)
        {
            try
            {
                if (solicitud == null ||
                    string.IsNullOrWhiteSpace(solicitud.IdentificacionCliente) ||
                    !CorreoValido(solicitud.CorreoCliente))
                {
                    return CrearRespuestaError("Debe indicar identificacion y correo valido.");
                }

                string identificacion = solicitud.IdentificacionCliente.Trim();
                string identificacionCifrada = ProveedorCryptoService.Encriptar(identificacion);
                string correo = solicitud.CorreoCliente.Trim();
                string connectionString = ObtenerConnectionString();

                const string sql = @"
UPDATE c
SET c.correo = @correo
FROM dbo.clientes c
WHERE c.identificacion = @identificacion
   OR c.identificacion = @identificacionCifrada
   OR EXISTS (
       SELECT 1
       FROM dbo.servicios s
       WHERE s.cliente_id = c.cliente_id
         AND (
             s.identificacion_dueno_cifrada = @identificacion
             OR s.identificacion_dueno_cifrada = @identificacionCifrada
         )
   );";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@identificacion", identificacion);
                    comando.Parameters.AddWithValue("@identificacionCifrada", identificacionCifrada);
                    comando.Parameters.AddWithValue("@correo", correo);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas <= 0)
                    {
                        return CrearRespuestaError("No se encontro el cliente en proveedor.");
                    }
                }

                return new RespuestaServicio
                {
                    Resultado = true,
                    Mensaje = "Correo del cliente sincronizado."
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ActualizarCorreoCliente: " + ex);
                return CrearRespuestaError("No fue posible sincronizar el correo del cliente.");
            }
        }

        public ListadoSolicitudesLineaResponse ListarSolicitudesLineaPendientes()
        {
            try
            {
                var solicitudes = new List<SolicitudLineaDto>();
                string connectionString = ObtenerConnectionString();

                const string sql = @"
SELECT
    solicitud_id,
    servicio_id,
    numero_telefono,
    tipo_servicio,
    identificacion_cliente,
    nombre_cliente,
    estado,
    fecha_solicitud
FROM dbo.solicitudes_linea
WHERE UPPER(estado) = 'PENDIENTE'
ORDER BY fecha_solicitud ASC, solicitud_id ASC;";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            solicitudes.Add(new SolicitudLineaDto
                            {
                                SolicitudId = Convert.ToInt32(reader["solicitud_id"]),
                                ServicioId = Convert.ToInt32(reader["servicio_id"]),
                                NumeroTelefono = Convert.ToString(reader["numero_telefono"]),
                                TipoServicio = Convert.ToString(reader["tipo_servicio"]),
                                IdentificacionCliente = Convert.ToString(reader["identificacion_cliente"]),
                                NombreCliente = Convert.ToString(reader["nombre_cliente"]),
                                Estado = Convert.ToString(reader["estado"]),
                                FechaSolicitud = Convert.ToDateTime(reader["fecha_solicitud"])
                                    .ToString("yyyy-MM-dd HH:mm:ss")
                            });
                        }
                    }
                }

                return new ListadoSolicitudesLineaResponse
                {
                    Resultado = true,
                    Mensaje = solicitudes.Count == 0
                        ? "No hay solicitudes pendientes."
                        : "Exitoso",
                    Solicitudes = solicitudes
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ListarSolicitudesLineaPendientes: " + ex);
                return new ListadoSolicitudesLineaResponse
                {
                    Resultado = false,
                    Mensaje = "No se pudieron consultar las solicitudes.",
                    Solicitudes = new List<SolicitudLineaDto>()
                };
            }
        }

        public RespuestaServicio MarcarSolicitudLineaAtendida(int solicitudId, string estado)
        {
            try
            {
                string estadoNormalizado = estado?.Trim().ToUpperInvariant();

                if (solicitudId <= 0 ||
                    (estadoNormalizado != "APROBADA" && estadoNormalizado != "RECHAZADA"))
                {
                    return CrearRespuestaError("Solicitud o estado incorrecto.");
                }

                string connectionString = ObtenerConnectionString();

                const string sql = @"
UPDATE dbo.solicitudes_linea
SET estado = @estado,
    fecha_atencion = SYSUTCDATETIME()
WHERE solicitud_id = @solicitudId
  AND UPPER(estado) = 'PENDIENTE';";

                using (var conexion = new SqlConnection(connectionString))
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@estado", estadoNormalizado);
                    comando.Parameters.AddWithValue("@solicitudId", solicitudId);
                    conexion.Open();

                    int afectadas = comando.ExecuteNonQuery();
                    if (afectadas == 0)
                    {
                        return CrearRespuestaError("La solicitud ya fue atendida o no existe.");
                    }
                }

                return new RespuestaServicio
                {
                    Resultado = true,
                    Mensaje = "Solicitud actualizada."
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en MarcarSolicitudLineaAtendida: " + ex);
                return CrearRespuestaError("No se pudo actualizar la solicitud.");
            }
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

                        if (ExisteIdentificadorServicio(
                            conexion,
                            transaccion,
                            solicitud.IdentificadorTelefono,
                            solicitud.IdentificadorTarjeta))
                        {
                            transaccion.Rollback();
                            return CrearRespuestaError("El identificador del telefono o de la tarjeta ya existe.");
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

                        EliminarDependenciasLineaDisponible(
                            conexion,
                            transaccion,
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

        private static FacturacionConsultaResponse CrearRespuestaErrorConsultaFacturacion(
            string detalle = null)
        {
            return new FacturacionConsultaResponse
            {
                Resultado = false,
                Mensaje = string.IsNullOrWhiteSpace(detalle)
                    ? "Problemas al consultar la facturacion."
                    : detalle.Trim(),
                NumeroTelefono = string.Empty,
                FechaCalculo = string.Empty,
                FechaMaximaPago = string.Empty,
                TotalLlamadas = 0,
                TotalFacturar = 0
            };
        }

        private static UltimaFacturacionResponse CrearRespuestaErrorUltimaFacturacion(
            string detalle = null)
        {
            return new UltimaFacturacionResponse
            {
                Resultado = false,
                Mensaje = string.IsNullOrWhiteSpace(detalle)
                    ? "Problemas al consultar el ultimo calculo."
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
    ISNULL(c.correo, '') AS correo_cliente,
    ISNULL(s.estado_linea, CASE WHEN s.activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END) AS estado_linea,
    s.activo,
    ISNULL(s.proveedor_codigo, 'KOLBI') AS proveedor_codigo
FROM dbo.servicios s
LEFT JOIN dbo.clientes c ON c.cliente_id = s.cliente_id
WHERE UPPER(ISNULL(s.estado_linea, CASE WHEN s.activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END)) = @estado
  AND (
      @estado <> 'DISPONIBLE'
      OR NOT EXISTS (
          SELECT 1
          FROM dbo.servicios activa
          WHERE activa.servicio_id <> s.servicio_id
            AND UPPER(ISNULL(activa.estado_linea, CASE WHEN activa.activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END)) = 'ACTIVO'
            AND (
                activa.numero_telefono = s.numero_telefono
                OR RIGHT(REPLACE(activa.numero_telefono, '+', ''), 8) = RIGHT(REPLACE(s.numero_telefono, '+', ''), 8)
            )
      )
  )
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
                                IdentificadorTelefonoVisible = DesencriptarOResumir(
                                    Convert.ToString(reader["identificador_telefono_cifrado"])),
                                IdentificadorTarjetaVisible = DesencriptarOResumir(
                                    Convert.ToString(reader["identificador_tarjeta_cifrado"])),
                                TipoServicio = Convert.ToString(reader["tipo_servicio"]),
                                IdentificacionCliente = Convert.ToString(reader["identificacion_dueno_cifrada"]),
                                IdentificacionClienteVisible = DesencriptarOResumir(
                                    Convert.ToString(reader["identificacion_dueno_cifrada"])),
                                NombreCliente = Convert.ToString(reader["nombre_cliente"]),
                                CorreoCliente = Convert.ToString(reader["correo_cliente"]),
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

        private static bool CorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return false;
            }

            return Regex.IsMatch(
                correo.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.CultureInvariant);
        }

        private static bool LineaDisponibleCoincide(
            SqlConnection conexion,
            SolicitarLineaClienteRequest solicitud)
        {
            const string sql = @"
SELECT COUNT(1)
FROM dbo.servicios
WHERE servicio_id = @servicioId
  AND numero_telefono = @numeroTelefono
  AND UPPER(ISNULL(tipo_servicio, '')) = @tipoServicio
  AND activo = 0
  AND UPPER(ISNULL(estado_linea, 'DISPONIBLE')) = 'DISPONIBLE';";

            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@servicioId", solicitud.ServicioId);
                comando.Parameters.AddWithValue("@numeroTelefono", solicitud.NumeroTelefono.Trim());
                comando.Parameters.AddWithValue("@tipoServicio", solicitud.TipoServicio.Trim().ToUpperInvariant());
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        private static bool ExisteSolicitudPendiente(
            SqlConnection conexion,
            int servicioId,
            string identificacionCliente)
        {
            const string sql = @"
SELECT COUNT(1)
FROM dbo.solicitudes_linea
WHERE servicio_id = @servicioId
  AND identificacion_cliente = @identificacionCliente
  AND UPPER(estado) = 'PENDIENTE';";

            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@servicioId", servicioId);
                comando.Parameters.AddWithValue("@identificacionCliente", identificacionCliente.Trim());
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        private static bool EsActivacion(string estado)
        {
            return string.Equals(
                estado?.Trim(),
                "activo",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool NumeroTieneLineaActiva(string numeroTelefonoCifrado)
        {
            string numeroTelefono = ProveedorCryptoService.Desencriptar(numeroTelefonoCifrado);

            if (string.IsNullOrWhiteSpace(numeroTelefono))
            {
                return false;
            }

            string connectionString = ObtenerConnectionString();

            const string sql = @"
SELECT COUNT(1)
FROM dbo.servicios
WHERE UPPER(ISNULL(estado_linea, CASE WHEN activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END)) = 'ACTIVO'
  AND (
      numero_telefono = @numeroTelefono
      OR RIGHT(REPLACE(numero_telefono, '+', ''), 8) = @ultimosOcho
  );";

            using (var conexion = new SqlConnection(connectionString))
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@numeroTelefono", numeroTelefono.Trim());
                comando.Parameters.AddWithValue("@ultimosOcho", UltimosOchoDigitos(numeroTelefono));
                conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        private static string UltimosOchoDigitos(string numeroTelefono)
        {
            if (string.IsNullOrWhiteSpace(numeroTelefono))
            {
                return string.Empty;
            }

            string digitos = string.Empty;

            foreach (char caracter in numeroTelefono)
            {
                if (char.IsDigit(caracter))
                {
                    digitos += caracter;
                }
            }

            return digitos.Length <= 8
                ? digitos
                : digitos.Substring(digitos.Length - 8);
        }

        private static bool ExisteTelefono(SqlConnection conexion, SqlTransaction transaccion, string numeroTelefono)
        {
            const string sql = @"
SELECT COUNT(1)
FROM dbo.servicios
WHERE numero_telefono = @numeroTelefono
   OR RIGHT(REPLACE(numero_telefono, '+', ''), 8) = @ultimosOcho;";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@numeroTelefono", numeroTelefono);
                comando.Parameters.AddWithValue("@ultimosOcho", UltimosOchoDigitos(numeroTelefono));
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        private static bool ExisteIdentificadorServicio(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string identificadorTelefono,
            string identificadorTarjeta)
        {
            string telefonoNormalizado = NormalizarIdentificadorAlmacenado(identificadorTelefono);
            string tarjetaNormalizada = NormalizarIdentificadorAlmacenado(identificadorTarjeta);

            const string sql = @"
SELECT
    ISNULL(identificador_telefono_cifrado, '') AS identificador_telefono_cifrado,
    ISNULL(identificador_tarjeta_cifrado, '') AS identificador_tarjeta_cifrado
FROM dbo.servicios;";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    string telefonoActual = Convert.ToString(reader["identificador_telefono_cifrado"]);
                    string tarjetaActual = Convert.ToString(reader["identificador_tarjeta_cifrado"]);

                    if (IdentificadorExistenteCoincide(telefonoActual, telefonoNormalizado, identificadorTelefono) ||
                        IdentificadorExistenteCoincide(tarjetaActual, tarjetaNormalizada, identificadorTarjeta))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IdentificadorExistenteCoincide(
            string valorActual,
            string valorNormalizado,
            string valorOriginal)
        {
            return ValoresIguales(valorActual, valorNormalizado) ||
                ValoresIguales(valorActual, valorOriginal) ||
                ValoresIguales(DesencriptarOResumir(valorActual), valorOriginal);
        }

        private static bool ValoresIguales(string valorActual, string valorNuevo)
        {
            return string.Equals(
                valorActual?.Trim(),
                valorNuevo?.Trim(),
                StringComparison.OrdinalIgnoreCase);
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
                comando.Parameters.AddWithValue(
                    "@identificadorTelefono",
                    NormalizarIdentificadorAlmacenado(solicitud.IdentificadorTelefono));
                comando.Parameters.AddWithValue(
                    "@identificadorTarjeta",
                    NormalizarIdentificadorAlmacenado(solicitud.IdentificadorTarjeta));
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        private static string NormalizarIdentificadorAlmacenado(string identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
            {
                return string.Empty;
            }

            string valor = identificador.Trim();

            if (valor.StartsWith("ENC_IMEI_", StringComparison.OrdinalIgnoreCase) ||
                valor.StartsWith("ENC_SIM_", StringComparison.OrdinalIgnoreCase))
            {
                return valor;
            }

            return ProveedorCryptoService.Encriptar(valor);
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

        private static void EliminarDependenciasLineaDisponible(
            SqlConnection conexion,
            SqlTransaction transaccion,
            int servicioId)
        {
            EjecutarSiExisteTabla(
                conexion,
                transaccion,
                "dbo.movimientos_saldo",
                "DELETE FROM dbo.movimientos_saldo WHERE servicio_id = @servicioId",
                servicioId);

            EjecutarSiExisteTabla(
                conexion,
                transaccion,
                "dbo.facturacion_postpago",
                "DELETE FROM dbo.facturacion_postpago WHERE servicio_id = @servicioId",
                servicioId);

            EjecutarSiExisteTabla(
                conexion,
                transaccion,
                "dbo.bitacora_proveedor",
                "DELETE FROM dbo.bitacora_proveedor WHERE servicio_id = @servicioId",
                servicioId);

            EjecutarSiExisteTabla(
                conexion,
                transaccion,
                "dbo.llamadas_proveedor",
                "DELETE FROM dbo.llamadas_proveedor WHERE servicio_id = @servicioId",
                servicioId);

            EjecutarSiExisteTabla(
                conexion,
                transaccion,
                "dbo.saldos",
                "DELETE FROM dbo.saldos WHERE servicio_id = @servicioId",
                servicioId);
        }

        private static void EjecutarSiExisteTabla(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string tabla,
            string sql,
            int servicioId)
        {
            if (!ExisteTabla(conexion, transaccion, tabla))
            {
                return;
            }

            EjecutarSinResultado(conexion, transaccion, sql, servicioId);
        }

        private static bool ExisteTabla(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string tabla)
        {
            const string sql = "SELECT CASE WHEN OBJECT_ID(@tabla, 'U') IS NULL THEN 0 ELSE 1 END";

            using (var comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@tabla", tabla);
                return Convert.ToInt32(comando.ExecuteScalar()) == 1;
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

        private static string DesencriptarOResumir(string valorCifrado)
        {
            string plano = ProveedorCryptoService.Desencriptar(valorCifrado);

            if (!string.IsNullOrWhiteSpace(plano))
            {
                return plano;
            }

            if (string.IsNullOrWhiteSpace(valorCifrado))
            {
                return string.Empty;
            }

            string valor = valorCifrado.Trim();
            if (valor.StartsWith("ENC_IMEI_", StringComparison.OrdinalIgnoreCase) ||
                valor.StartsWith("ENC_SIM_", StringComparison.OrdinalIgnoreCase))
            {
                return valor;
            }

            return valor.Length <= 12
                ? valor
                : valor.Substring(0, 8) + "...";
        }
    }
}
