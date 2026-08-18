using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WS_ProveedorCliente.Models;

namespace WS_ProveedorCliente.Services
{
    public class LineaClienteService
    {
        private readonly string _connectionString;
        private readonly int _timeoutSegundos;

        public LineaClienteService(string connectionString, int timeoutSegundos)
        {
            _connectionString = connectionString;
            _timeoutSegundos = timeoutSegundos > 0 ? timeoutSegundos : 10;
        }

        public ConsultarLineasClienteResponse Consultar(string identificacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    return new ConsultarLineasClienteResponse
                    {
                        Resultado = false,
                        Mensaje = "La identificacion es obligatoria.",
                        Lineas = new List<LineaClienteDto>()
                    };
                }

                string identificacionCifrada = CryptoAes.Encriptar(identificacion.Trim());

                var lineas = new List<LineaClienteDto>();

                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand(ConsultaSql, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.CommandTimeout = _timeoutSegundos;
                    comando.Parameters.AddWithValue(
                        "@identificacionCifrada",
                        (object)identificacionCifrada ?? DBNull.Value);

                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lineas.Add(new LineaClienteDto
                            {
                                NumeroTelefono = Convert.ToString(lector["numero_telefono"]),
                                TipoServicio = Convert.ToString(lector["tipo_servicio"]),
                                Saldo = Convert.ToDecimal(lector["saldo"]),
                                FacturaPendiente = Convert.ToDecimal(lector["factura_pendiente"]),
                                FacturaFechaMaximaPago = lector["factura_fecha_maxima_pago"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(lector["factura_fecha_maxima_pago"]),
                                Activo = Convert.ToBoolean(lector["activo"]),
                                IdentificadorTelefono = Convert.ToString(lector["identificador_telefono_cifrado"]),
                                IdentificadorTarjeta = Convert.ToString(lector["identificador_tarjeta_cifrado"]),
                                IdentificacionDuenoCifrada = Convert.ToString(lector["identificacion_dueno_cifrada"])
                            });
                        }
                    }
                }

                return new ConsultarLineasClienteResponse
                {
                    Resultado = true,
                    Mensaje = lineas.Count == 0
                        ? "El cliente no posee lineas activas."
                        : "Consulta exitosa.",
                    Lineas = lineas
                };
            }
            catch (Exception ex)
            {
                return new ConsultarLineasClienteResponse
                {
                    Resultado = false,
                    Mensaje = "Error al consultar las lineas del cliente: " + ex.Message,
                    Lineas = new List<LineaClienteDto>()
                };
            }
        }

        public RecargarSaldoResponse Recargar(string numeroTelefono, decimal monto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroTelefono))
                {
                    return ErrorRecarga("El numero de telefono es obligatorio.");
                }

                if (monto <= 0)
                {
                    return ErrorRecarga("El monto a cargar debe ser positivo y sin decimales.");
                }

                using (SqlConnection conexion = new SqlConnection(_connectionString))
                {
                    conexion.Open();

                    using (SqlTransaction transaccion = conexion.BeginTransaction())
                    {
                        int servicioId = BuscarServicioPrepagoActivo(
                            conexion,
                            transaccion,
                            numeroTelefono.Trim());

                        if (servicioId <= 0)
                        {
                            transaccion.Rollback();
                            return ErrorRecarga(
                                "La linea prepago no existe o no esta activa.");
                        }

                        decimal nuevoSaldo = SumarSaldo(
                            conexion,
                            transaccion,
                            servicioId,
                            monto);

                        transaccion.Commit();

                        return new RecargarSaldoResponse
                        {
                            Resultado = true,
                            Mensaje = "Registro exitoso",
                            NuevoSaldo = nuevoSaldo
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorRecarga(
                    "Error al cargar el saldo: " + ex.Message);
            }
        }

        private int BuscarServicioPrepagoActivo(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string numeroTelefono)
        {
            const string sql = @"
SELECT TOP 1 s.servicio_id
FROM dbo.servicios s
WHERE s.numero_telefono = @numeroTelefono
  AND UPPER(ISNULL(s.tipo_servicio, '')) = 'PREPAGO'
  AND s.activo = 1
  AND UPPER(ISNULL(s.estado_linea, 'ACTIVO')) = 'ACTIVO'";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.CommandTimeout = _timeoutSegundos;
                comando.Parameters.AddWithValue(
                    "@numeroTelefono",
                    (object)numeroTelefono ?? DBNull.Value);

                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value
                    ? 0
                    : Convert.ToInt32(valor);
            }
        }

        private decimal SumarSaldo(
            SqlConnection conexion,
            SqlTransaction transaccion,
            int servicioId,
            decimal monto)
        {
            const string actualizar = @"
UPDATE dbo.saldos
SET saldo_disponible = saldo_disponible + @monto,
    fecha_actualizacion = GETDATE()
WHERE servicio_id = @servicioId";

            using (SqlCommand comando = new SqlCommand(actualizar, conexion, transaccion))
            {
                comando.CommandTimeout = _timeoutSegundos;
                comando.Parameters.AddWithValue("@monto", monto);
                comando.Parameters.AddWithValue("@servicioId", servicioId);

                if (comando.ExecuteNonQuery() == 0)
                {
                    const string insertar = @"
INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
VALUES (@servicioId, @monto)";

                    using (SqlCommand insertarComando = new SqlCommand(insertar, conexion, transaccion))
                    {
                        insertarComando.CommandTimeout = _timeoutSegundos;
                        insertarComando.Parameters.AddWithValue("@monto", monto);
                        insertarComando.Parameters.AddWithValue("@servicioId", servicioId);
                        insertarComando.ExecuteNonQuery();
                    }
                }
            }

            const string obtener = @"
SELECT saldo_disponible
FROM dbo.saldos
WHERE servicio_id = @servicioId";

            using (SqlCommand comando = new SqlCommand(obtener, conexion, transaccion))
            {
                comando.CommandTimeout = _timeoutSegundos;
                comando.Parameters.AddWithValue("@servicioId", servicioId);
                return Convert.ToDecimal(comando.ExecuteScalar());
            }
        }

        public PagarFacturaResponse PagarFactura(
            string numeroTelefono,
            decimal monto,
            string identificacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroTelefono))
                {
                    return ErrorPago("El numero de telefono es obligatorio.");
                }

                if (monto <= 0)
                {
                    return ErrorPago("El monto de la factura debe ser mayor a cero.");
                }

                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    return ErrorPago("La identificacion del cliente es obligatoria.");
                }

                using (SqlConnection conexion = new SqlConnection(_connectionString))
                {
                    conexion.Open();

                    using (SqlTransaction transaccion = conexion.BeginTransaction())
                    {
                        int servicioId = BuscarServicioPostpagoActivo(
                            conexion,
                            transaccion,
                            numeroTelefono.Trim());

                        if (servicioId <= 0)
                        {
                            transaccion.Rollback();
                            return ErrorPago(
                                "La linea postpago no existe o no esta activa.");
                        }

                        int facturacionId;
                        decimal pendiente;

                        using (SqlCommand comando = new SqlCommand(PendienteSql, conexion, transaccion))
                        {
                            comando.CommandTimeout = _timeoutSegundos;
                            comando.Parameters.AddWithValue("@servicioId", servicioId);

                            using (SqlDataReader lector = comando.ExecuteReader())
                            {
                                if (!lector.Read())
                                {
                                    lector.Close();
                                    transaccion.Rollback();
                                    return ErrorPago(
                                        "La linea postpago no posee factura pendiente.");
                                }

                                facturacionId = Convert.ToInt32(lector["facturacion_id"]);
                                pendiente = Convert.ToDecimal(lector["total_facturar"]);
                            }
                        }

                        if (monto != pendiente)
                        {
                            transaccion.Rollback();
                            return ErrorPago(
                                "El monto no coincide con el monto pendiente de la factura.");
                        }

                        using (SqlCommand comando = new SqlCommand(CancelarSql, conexion, transaccion))
                        {
                            comando.CommandTimeout = _timeoutSegundos;
                            comando.Parameters.AddWithValue("@facturacionId", facturacionId);
                            comando.ExecuteNonQuery();
                        }

                        string correo = ObtenerCorreoCliente(
                            conexion,
                            transaccion,
                            identificacion.Trim());

                        transaccion.Commit();

                        return new PagarFacturaResponse
                        {
                            Resultado = true,
                            Mensaje = "Registro exitoso",
                            CorreoCliente = correo,
                            FacturacionId = facturacionId,
                            MontoCancelado = pendiente,
                            FechaPago = DateTime.Now
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorPago(
                    "Error al cancelar la factura: " + ex.Message);
            }
        }

        private int BuscarServicioPostpagoActivo(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string numeroTelefono)
        {
            const string sql = @"
SELECT TOP 1 s.servicio_id
FROM dbo.servicios s
WHERE s.numero_telefono = @numeroTelefono
  AND UPPER(ISNULL(s.tipo_servicio, '')) = 'POSTPAGO'
  AND s.activo = 1
  AND UPPER(ISNULL(s.estado_linea, 'ACTIVO')) = 'ACTIVO'";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.CommandTimeout = _timeoutSegundos;
                comando.Parameters.AddWithValue(
                    "@numeroTelefono",
                    (object)numeroTelefono ?? DBNull.Value);

                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value
                    ? 0
                    : Convert.ToInt32(valor);
            }
        }

        private string ObtenerCorreoCliente(
            SqlConnection conexion,
            SqlTransaction transaccion,
            string identificacion)
        {
            const string sql = @"
SELECT TOP 1 correo
FROM dbo.clientes
WHERE identificacion = @identificacion
  AND correo IS NOT NULL
  AND correo <> ''";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.CommandTimeout = _timeoutSegundos;
                comando.Parameters.AddWithValue("@identificacion", identificacion);

                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(valor);
            }
        }

        private PagarFacturaResponse ErrorPago(string mensaje) =>
            new PagarFacturaResponse
            {
                Resultado = false,
                Mensaje = mensaje,
                CorreoCliente = string.Empty,
                FacturacionId = 0,
                MontoCancelado = 0,
                FechaPago = DateTime.Now
            };

        private const string PendienteSql = @"
SELECT TOP 1 facturacion_id, total_facturar
FROM dbo.facturacion_postpago
WHERE servicio_id = @servicioId
  AND total_facturar > 0
ORDER BY fecha_calculo DESC, facturacion_id DESC";

        private const string CancelarSql = @"
UPDATE dbo.facturacion_postpago
SET total_facturar = 0.00,
    fecha_registro = GETDATE()
WHERE facturacion_id = @facturacionId";

        private RecargarSaldoResponse ErrorRecarga(string mensaje) =>
            new RecargarSaldoResponse
            {
                Resultado = false,
                Mensaje = mensaje,
                NuevoSaldo = 0
            };

        private const string ConsultaSql = @"
SELECT
    s.numero_telefono,
    s.tipo_servicio,
    s.activo,
    ISNULL(sd.saldo_disponible, 0)                          AS saldo,
    ISNULL(factura.total_facturar, 0)                        AS factura_pendiente,
    factura.fecha_maxima_pago                               AS factura_fecha_maxima_pago,
    ISNULL(s.identificador_telefono_cifrado, '')            AS identificador_telefono_cifrado,
    ISNULL(s.identificador_tarjeta_cifrado, '')             AS identificador_tarjeta_cifrado,
    ISNULL(s.identificacion_dueno_cifrada, '')              AS identificacion_dueno_cifrada
FROM dbo.servicios s
LEFT JOIN dbo.saldos sd
    ON sd.servicio_id = s.servicio_id
OUTER APPLY (
    SELECT TOP 1 f.total_facturar, f.fecha_maxima_pago
    FROM dbo.facturacion_postpago f
    WHERE f.servicio_id = s.servicio_id
    ORDER BY f.fecha_calculo DESC, f.facturacion_id DESC
) AS factura
WHERE s.identificacion_dueno_cifrada = @identificacionCifrada
  AND s.activo = 1
  AND UPPER(ISNULL(s.estado_linea, 'ACTIVO')) = 'ACTIVO'";
    }
}