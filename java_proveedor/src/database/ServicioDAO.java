package java_proveedor.src.database;

import java_proveedor.src.models.Servicio;
import java.math.BigDecimal;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;

public class ServicioDAO {

    private String ultimoError = "";

    public String getUltimoError() {
        return ultimoError;
    }

    public Servicio obtenerDetalleParaLlamada(String numeroTelefono, String tipoDestino) {
        ultimoError = "";

        String sql =
            "SELECT s.servicio_id, s.cliente_id, s.numero_telefono, s.tipo_servicio, s.activo, " +
            "sa.saldo_disponible, t.costo_por_minuto " +
            "FROM servicios s " +
            "LEFT JOIN saldos sa ON s.servicio_id = sa.servicio_id " +
            "LEFT JOIN tarifas t ON t.tipo_llamada = ? " +
            "WHERE s.numero_telefono = ? " +
            "OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, tipoDestino);
            ps.setString(2, numeroTelefono);
            ps.setString(3, ultimosOchoDigitos(numeroTelefono));

            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) {
                    Servicio servicio = new Servicio();
                    servicio.setServicioId(rs.getInt("servicio_id"));
                    servicio.setClienteId(rs.getInt("cliente_id"));
                    servicio.setNumeroTelefono(rs.getString("numero_telefono"));
                    servicio.setTipoServicio(rs.getString("tipo_servicio"));
                    servicio.setActivo(rs.getBoolean("activo"));

                    BigDecimal saldo = rs.getBigDecimal("saldo_disponible");
                    servicio.setSaldoDisponible(saldo != null ? saldo : BigDecimal.ZERO);

                    BigDecimal tarifa = rs.getBigDecimal("costo_por_minuto");
                    servicio.setTarifaAplicable(
                        tarifa != null ? tarifa : BigDecimal.valueOf(10.0)
                    );

                    return servicio;
                }
            }
        } catch (Exception e) {
            ultimoError = e.getMessage();
            System.err.println(
                "[Error ServicioDAO] No se pudo consultar SQL Server para telefono: "
                    + numeroTelefono
            );
            e.printStackTrace();
        }

        return null;
    }

    public String obtenerDetalleAdministrativoJson(String numeroTelefono) {
        ultimoError = "";

        String sql =
            "SELECT s.servicio_id, s.cliente_id, c.nombre AS cliente_nombre, " +
            "s.numero_telefono, s.tipo_servicio, s.activo, " +
            "COALESCE(s.proveedor_codigo, 'KOLBI') AS proveedor_codigo, " +
            "COALESCE(sa.saldo_disponible, 0) AS saldo_disponible, " +
            "COALESCE(p.nombre, 'Costa Rica') AS pais, " +
            "COALESCE(p.codigo_area, '+506') AS codigo_area, " +
            "COALESCE(p.clasificacion, 'NACIONAL') AS nacionalidad, " +
            "COALESCE(t.tipo_llamada, CASE WHEN COALESCE(p.clasificacion, 'NACIONAL') = 'NACIONAL' THEN 'NACIONAL' ELSE 'INTERNACIONAL' END) AS tipo_llamada " +
            "FROM servicios s " +
            "JOIN clientes c ON c.cliente_id = s.cliente_id " +
            "LEFT JOIN saldos sa ON s.servicio_id = sa.servicio_id " +
            "LEFT JOIN paises p ON p.pais_id = s.pais_id " +
            "LEFT JOIN tarifas t ON t.pais_id = p.pais_id AND t.activa = 1 " +
            "WHERE s.numero_telefono = ? " +
            "OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, numeroTelefono);
            ps.setString(2, ultimosOchoDigitos(numeroTelefono));

            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) {
                    String codigo = rs.getString("proveedor_codigo");
                    return "{"
                        + "\"servicio_id\":" + rs.getInt("servicio_id") + ","
                        + "\"cliente\":\"" + sanitizar(rs.getString("cliente_nombre")) + "\","
                        + "\"numero\":\"" + sanitizar(rs.getString("numero_telefono")) + "\","
                        + "\"tipo_servicio\":\"" + sanitizar(rs.getString("tipo_servicio")) + "\","
                        + "\"activo\":" + rs.getBoolean("activo") + ","
                        + "\"proveedor_codigo\":\"" + sanitizar(codigo) + "\","
                        + "\"proveedor\":\"" + sanitizar(nombreProveedor(codigo)) + "\","
                        + "\"pais\":\"" + sanitizar(rs.getString("pais")) + "\","
                        + "\"codigo_area\":\"" + sanitizar(rs.getString("codigo_area")) + "\","
                        + "\"nacionalidad\":\"" + sanitizar(rs.getString("nacionalidad")) + "\","
                        + "\"tipo_llamada\":\"" + sanitizar(rs.getString("tipo_llamada")) + "\","
                        + "\"saldo\":\"" + rs.getBigDecimal("saldo_disponible").toString() + "\""
                        + "}";
                }
            }
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
        }

        return "";
    }

    public boolean recargarSaldo(String numeroTelefono, BigDecimal monto) {
        ultimoError = "";

        String sql =
            "UPDATE sa SET sa.saldo_disponible = sa.saldo_disponible + ?, " +
            "sa.fecha_actualizacion = GETDATE() " +
            "FROM saldos sa " +
            "JOIN servicios s ON s.servicio_id = sa.servicio_id " +
            "WHERE s.numero_telefono = ? " +
            "OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setBigDecimal(1, monto);
            ps.setString(2, numeroTelefono);
            ps.setString(3, ultimosOchoDigitos(numeroTelefono));
            return ps.executeUpdate() > 0;
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
            return false;
        }
    }

    public boolean cambiarEstadoTelefono(String numeroTelefono, boolean activo) {
        ultimoError = "";

        String sql =
            "UPDATE servicios SET activo = ? " +
            "WHERE numero_telefono = ? " +
            "OR RIGHT(REPLACE(numero_telefono, '+', ''), 8) = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setBoolean(1, activo);
            ps.setString(2, numeroTelefono);
            ps.setString(3, ultimosOchoDigitos(numeroTelefono));
            return ps.executeUpdate() > 0;
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
            return false;
        }
    }

    public boolean registrarTelefono(
        String numeroTelefono,
        String tipoServicio,
        String proveedorCodigo,
        BigDecimal saldoInicial,
        boolean activo
    ) {
        ultimoError = "";

        if (existeTelefono(numeroTelefono)) {
            ultimoError = "El numero ya existe en SQL Server";
            return false;
        }

        Connection conn = null;

        try {
            conn = ConexionSQL.getConexion();
            conn.setAutoCommit(false);

            int clienteId;
            try (
                PreparedStatement cliente = conn.prepareStatement(
                    "INSERT INTO clientes (nombre, identificacion, correo, activo) VALUES (?, ?, ?, 1)",
                    Statement.RETURN_GENERATED_KEYS
                )
            ) {
                cliente.setString(1, "Cliente " + numeroTelefono);
                cliente.setString(2, "ID-" + numeroTelefono);
                cliente.setString(3, numeroTelefono + "@central.test");
                cliente.executeUpdate();

                try (ResultSet keys = cliente.getGeneratedKeys()) {
                    if (!keys.next()) {
                        throw new IllegalStateException("No se genero cliente_id");
                    }
                    clienteId = keys.getInt(1);
                }
            }

            int servicioId;
            try (
                PreparedStatement servicio = conn.prepareStatement(
                    "INSERT INTO servicios (cliente_id, numero_telefono, tipo_servicio, activo, proveedor_codigo) " +
                    "VALUES (?, ?, ?, ?, ?)",
                    Statement.RETURN_GENERATED_KEYS
                )
            ) {
                servicio.setInt(1, clienteId);
                servicio.setString(2, numeroTelefono);
                servicio.setString(3, tipoServicio);
                servicio.setBoolean(4, activo);
                servicio.setString(5, proveedorCodigo);
                servicio.executeUpdate();

                try (ResultSet keys = servicio.getGeneratedKeys()) {
                    if (!keys.next()) {
                        throw new IllegalStateException("No se genero servicio_id");
                    }
                    servicioId = keys.getInt(1);
                }
            }

            try (
                PreparedStatement saldo = conn.prepareStatement(
                    "INSERT INTO saldos (servicio_id, saldo_disponible) VALUES (?, ?)"
                )
            ) {
                saldo.setInt(1, servicioId);
                saldo.setBigDecimal(2, saldoInicial);
                saldo.executeUpdate();
            }

            conn.commit();
            return true;
        } catch (Exception e) {
            ultimoError = e.getMessage();
            if (conn != null) {
                try {
                    conn.rollback();
                } catch (Exception rollbackError) {
                    System.err.println("[Error ServicioDAO] No se pudo revertir registro de telefono");
                    rollbackError.printStackTrace();
                }
            }
            e.printStackTrace();
            return false;
        } finally {
            if (conn != null) {
                try {
                    conn.setAutoCommit(true);
                    conn.close();
                } catch (Exception closeError) {
                    System.err.println("[Error ServicioDAO] No se pudo cerrar conexion de registro");
                    closeError.printStackTrace();
                }
            }
        }
    }

    public LineaProveedor5 obtenerLineaProveedor5(String numeroTelefono) {
        ultimoError = "";

        String sql =
            "SELECT servicio_id, numero_telefono, tipo_servicio, activo, " +
            "COALESCE(estado_linea, CASE WHEN activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END) AS estado_linea, " +
            "identificador_telefono_cifrado, identificador_tarjeta_cifrado, identificacion_dueno_cifrada " +
            "FROM servicios " +
            "WHERE numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, numeroTelefono);

            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) {
                    LineaProveedor5 linea = new LineaProveedor5();
                    linea.servicioId = rs.getInt("servicio_id");
                    linea.numeroTelefono = rs.getString("numero_telefono");
                    linea.tipoServicio = rs.getString("tipo_servicio");
                    linea.activo = rs.getBoolean("activo");
                    linea.estadoLinea = rs.getString("estado_linea");
                    linea.identificadorTelefono = rs.getString("identificador_telefono_cifrado");
                    linea.identificadorTarjeta = rs.getString("identificador_tarjeta_cifrado");
                    linea.identificacionDueno = rs.getString("identificacion_dueno_cifrada");
                    return linea;
                }
            }
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
        }

        return null;
    }

    public boolean activarLineaProveedor5(
        int servicioId,
        String identificadorTelefono,
        String identificadorTarjeta,
        String tipoServicio,
        String identificacionDueno,
        BigDecimal saldoInicialPrepago
    ) {
        ultimoError = "";
        Connection conn = null;

        try {
            conn = ConexionSQL.getConexion();
            conn.setAutoCommit(false);

            try (
                PreparedStatement servicio = conn.prepareStatement(
                    "UPDATE servicios " +
                    "SET activo = 1, estado_linea = 'ACTIVO', tipo_servicio = ?, " +
                    "identificador_telefono_cifrado = ?, identificador_tarjeta_cifrado = ?, " +
                    "identificacion_dueno_cifrada = ? " +
                    "WHERE servicio_id = ?"
                )
            ) {
                servicio.setString(1, tipoServicio);
                servicio.setString(2, identificadorTelefono);
                servicio.setString(3, identificadorTarjeta);
                servicio.setString(4, identificacionDueno);
                servicio.setInt(5, servicioId);

                if (servicio.executeUpdate() == 0) {
                    throw new IllegalStateException("No se actualizo el servicio");
                }
            }

            if ("PREPAGO".equalsIgnoreCase(tipoServicio)) {
                asegurarSaldoPrepago(conn, servicioId, saldoInicialPrepago);
            }

            conn.commit();
            return true;
        } catch (Exception e) {
            ultimoError = e.getMessage();
            if (conn != null) {
                try {
                    conn.rollback();
                } catch (Exception rollbackError) {
                    rollbackError.printStackTrace();
                }
            }
            e.printStackTrace();
            return false;
        } finally {
            cerrarConexionTransaccional(conn);
        }
    }

    public boolean desactivarLineaProveedor5(int servicioId) {
        ultimoError = "";

        String sql =
            "UPDATE servicios " +
            "SET activo = 0, estado_linea = 'DISPONIBLE', identificacion_dueno_cifrada = NULL " +
            "WHERE servicio_id = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setInt(1, servicioId);
            return ps.executeUpdate() > 0;
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
            return false;
        }
    }

    private void asegurarSaldoPrepago(
        Connection conn,
        int servicioId,
        BigDecimal saldoInicialPrepago
    ) throws Exception {
        try (
            PreparedStatement existe = conn.prepareStatement(
                "SELECT COUNT(1) FROM saldos WHERE servicio_id = ?"
            )
        ) {
            existe.setInt(1, servicioId);

            try (ResultSet rs = existe.executeQuery()) {
                if (rs.next() && rs.getInt(1) > 0) {
                    try (
                        PreparedStatement update = conn.prepareStatement(
                            "UPDATE saldos SET saldo_disponible = ?, fecha_actualizacion = GETDATE() " +
                            "WHERE servicio_id = ?"
                        )
                    ) {
                        update.setBigDecimal(1, saldoInicialPrepago);
                        update.setInt(2, servicioId);
                        update.executeUpdate();
                    }
                    return;
                }
            }
        }

        try (
            PreparedStatement insert = conn.prepareStatement(
                "INSERT INTO saldos (servicio_id, saldo_disponible) VALUES (?, ?)"
            )
        ) {
            insert.setInt(1, servicioId);
            insert.setBigDecimal(2, saldoInicialPrepago);
            insert.executeUpdate();
        }
    }

    private void cerrarConexionTransaccional(Connection conn) {
        if (conn == null) {
            return;
        }

        try {
            conn.setAutoCommit(true);
            conn.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static class LineaProveedor5 {
        public int servicioId;
        public String numeroTelefono;
        public String tipoServicio;
        public boolean activo;
        public String estadoLinea;
        public String identificadorTelefono;
        public String identificadorTarjeta;
        public String identificacionDueno;
    }

    private boolean existeTelefono(String numeroTelefono) {
        String sql =
            "SELECT COUNT(1) AS total FROM servicios " +
            "WHERE numero_telefono = ? " +
            "OR RIGHT(REPLACE(numero_telefono, '+', ''), 8) = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, numeroTelefono);
            ps.setString(2, ultimosOchoDigitos(numeroTelefono));

            try (ResultSet rs = ps.executeQuery()) {
                return rs.next() && rs.getInt("total") > 0;
            }
        } catch (Exception e) {
            ultimoError = e.getMessage();
            e.printStackTrace();
            return true;
        }
    }

    private String nombreProveedor(String codigo) {
        if (codigo == null) {
            return "Proveedor no disponible";
        }

        switch (codigo.toUpperCase()) {
            case "KOLBI":
                return "Kolbi";
            case "CLARO":
                return "Claro";
            case "LIBERTY":
                return "Liberty";
            case "MOVISTAR":
                return "Movistar";
            case "XYZ":
                return "Proveedor Telefonico XYZ";
            default:
                return codigo;
        }
    }

    private String ultimosOchoDigitos(String numeroTelefono) {
        if (numeroTelefono == null) {
            return "";
        }

        String soloDigitos = numeroTelefono.replaceAll("\\D", "");

        if (soloDigitos.length() <= 8) {
            return soloDigitos;
        }

        return soloDigitos.substring(soloDigitos.length() - 8);
    }

    private String sanitizar(String texto) {
        if (texto == null) {
            return "";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
