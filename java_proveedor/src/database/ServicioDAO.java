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
            "WHERE s.numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, tipoDestino);
            ps.setString(2, numeroTelefono);

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
            "COALESCE(sa.saldo_disponible, 0) AS saldo_disponible " +
            "FROM servicios s " +
            "JOIN clientes c ON c.cliente_id = s.cliente_id " +
            "LEFT JOIN saldos sa ON s.servicio_id = sa.servicio_id " +
            "WHERE s.numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, numeroTelefono);

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
            "WHERE s.numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setBigDecimal(1, monto);
            ps.setString(2, numeroTelefono);
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
            "UPDATE servicios SET activo = ? WHERE numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setBoolean(1, activo);
            ps.setString(2, numeroTelefono);
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

    private boolean existeTelefono(String numeroTelefono) {
        String sql = "SELECT COUNT(1) AS total FROM servicios WHERE numero_telefono = ?";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, numeroTelefono);

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

    private String sanitizar(String texto) {
        if (texto == null) {
            return "";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
