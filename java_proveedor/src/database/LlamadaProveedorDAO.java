package java_proveedor.src.database;

import java_proveedor.src.models.Servicio;
import java_proveedor.src.models.Tarifa;

import java.math.BigDecimal;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Statement;
import java.sql.Timestamp;
import java.sql.Date;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class LlamadaProveedorDAO
{
    public LlamadaProveedorDAO() {}

    public ResultadoRegistro registrarMovimiento(
        Servicio servicio,
        Tarifa tarifa,
        String telefonoDestino,
        LocalDateTime fechaInicio,
        LocalDateTime fechaFin,
        int duracionSegundos,
        int duracionMinutos,
        String motivoFinalizacion,
        BigDecimal montoTotal,
        String moneda
    ) {
        try (Connection conn = ConexionSQL.getConexion()) {
            conn.setAutoCommit(false);

            try {
                BigDecimal saldoAnterior = obtenerSaldo(conn, servicio.getServicioId());
                BigDecimal saldoPosterior = saldoAnterior;

                if (esPrepago(servicio)) {
                    if (saldoAnterior.compareTo(montoTotal) < 0) {
                        if (esCierrePorSaldoAgotado(motivoFinalizacion)) {
                            montoTotal = saldoAnterior;
                        } else {
                            conn.rollback();
                            return ResultadoRegistro.error("Saldo insuficiente para registrar el rebajo");
                        }
                    }

                    saldoPosterior = saldoAnterior.subtract(montoTotal);
                    actualizarSaldo(conn, servicio.getServicioId(), saldoPosterior);
                }

                int llamadaId = insertarLlamada(
                    conn,
                    servicio,
                    tarifa,
                    telefonoDestino,
                    fechaInicio,
                    fechaFin,
                    duracionSegundos,
                    montoTotal
                );

                insertarMovimiento(
                    conn,
                    servicio,
                    llamadaId,
                    esPrepago(servicio) ? "REBAJO_SALDO" : "COBRO_POSTPAGO",
                    montoTotal,
                    descripcionMovimiento(telefonoDestino, motivoFinalizacion, moneda)
                );

                conn.commit();

                return ResultadoRegistro.ok(
                    llamadaId,
                    saldoAnterior,
                    saldoPosterior,
                    montoTotal
                );
            } catch (Exception e) {
                conn.rollback();
                return ResultadoRegistro.error(e.getMessage());
            } finally {
                conn.setAutoCommit(true);
            }
        } catch (Exception e) {
            return ResultadoRegistro.error(e.getMessage());
        }
    }

    private int insertarLlamada(
        Connection conn,
        Servicio servicio,
        Tarifa tarifa,
        String telefonoDestino,
        LocalDateTime fechaInicio,
        LocalDateTime fechaFin,
        int duracionSegundos,
        BigDecimal montoTotal
    ) throws Exception {
        String sql =
            "INSERT INTO llamadas_proveedor " +
            "(servicio_id, tarifa_id, telefono_destino, fecha_llamada, hora_llamada, costo, duracion) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?)";

        try (PreparedStatement ps = conn.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
            ps.setInt(1, servicio.getServicioId());
            ps.setInt(2, tarifa.getTarifaId());
            ps.setString(3, telefonoDestino);
            ps.setDate(4, Date.valueOf(fechaInicio.toLocalDate()));
            ps.setString(5, fechaInicio.format(DateTimeFormatter.ofPattern("HHmmss")));
            ps.setBigDecimal(6, montoTotal);
            ps.setString(7, formatearDuracion(duracionSegundos));
            ps.executeUpdate();

            try (ResultSet rs = ps.getGeneratedKeys()) {
                if (rs.next()) {
                    return rs.getInt(1);
                }
            }
        }

        throw new IllegalStateException("No se pudo obtener el ID de la llamada registrada");
    }

    private void insertarMovimiento(
        Connection conn,
        Servicio servicio,
        int llamadaId,
        String tipoMovimiento,
        BigDecimal montoTotal,
        String descripcion
    ) throws Exception {
        String sql =
            "INSERT INTO movimientos_saldo " +
            "(servicio_id, llamada_id, tipo_movimiento, monto, fecha_movimiento, descripcion) " +
            "VALUES (?, ?, ?, ?, ?, ?)";

        try (PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setInt(1, servicio.getServicioId());
            ps.setInt(2, llamadaId);
            ps.setString(3, tipoMovimiento);
            ps.setBigDecimal(4, montoTotal);
            ps.setTimestamp(5, Timestamp.valueOf(LocalDateTime.now()));
            ps.setString(6, descripcion);
            ps.executeUpdate();
        }
    }

    private BigDecimal obtenerSaldo(Connection conn, int servicioId) throws Exception
    {
        String sql = "SELECT saldo_disponible FROM saldos WHERE servicio_id = ?";

        try (PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setInt(1, servicioId);

            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) {
                    BigDecimal saldo = rs.getBigDecimal("saldo_disponible");
                    return saldo != null ? saldo : BigDecimal.ZERO;
                }
            }
        }

        return BigDecimal.ZERO;
    }

    private void actualizarSaldo(Connection conn, int servicioId, BigDecimal saldoPosterior)
        throws Exception
    {
        String sql =
            "UPDATE saldos " +
            "SET saldo_disponible = ?, fecha_actualizacion = GETDATE() " +
            "WHERE servicio_id = ?";

        try (PreparedStatement ps = conn.prepareStatement(sql)) {
            ps.setBigDecimal(1, saldoPosterior);
            ps.setInt(2, servicioId);
            int actualizados = ps.executeUpdate();

            if (actualizados == 0) {
                throw new IllegalStateException("No se encontro saldo para actualizar");
            }
        }
    }

    private boolean esPrepago(Servicio servicio)
    {
        return "PREPAGO".equalsIgnoreCase(servicio.getTipoServicio());
    }

    private boolean esCierrePorSaldoAgotado(String motivoFinalizacion)
    {
        return "SALDO_AGOTADO".equalsIgnoreCase(motivoFinalizacion);
    }

    private String formatearDuracion(int duracionSegundos)
    {
        int segundos = Math.max(0, duracionSegundos);
        int horas = segundos / 3600;
        int minutos = (segundos % 3600) / 60;
        int restoSegundos = segundos % 60;
        return String.format("%02d:%02d:%02d", horas, minutos, restoSegundos);
    }

    private String descripcionMovimiento(
        String telefonoDestino,
        String motivoFinalizacion,
        String moneda
    ) {
        String destino = telefonoDestino == null || telefonoDestino.isBlank()
            ? "destino no indicado"
            : telefonoDestino;
        String motivo = motivoFinalizacion == null || motivoFinalizacion.isBlank()
            ? "FINALIZACION"
            : motivoFinalizacion;
        String monedaMovimiento = moneda == null || moneda.isBlank()
            ? "CRC"
            : moneda;

        return "Movimiento por llamada a " + destino + " (" + motivo + ") en " + monedaMovimiento;
    }

    public static class ResultadoRegistro
    {
        private final boolean exitoso;
        private final String mensaje;
        private final int llamadaId;
        private final BigDecimal saldoAnterior;
        private final BigDecimal saldoPosterior;
        private final BigDecimal montoRebajado;

        private ResultadoRegistro(
            boolean exitoso,
            String mensaje,
            int llamadaId,
            BigDecimal saldoAnterior,
            BigDecimal saldoPosterior,
            BigDecimal montoRebajado
        ) {
            this.exitoso = exitoso;
            this.mensaje = mensaje;
            this.llamadaId = llamadaId;
            this.saldoAnterior = saldoAnterior;
            this.saldoPosterior = saldoPosterior;
            this.montoRebajado = montoRebajado;
        }

        public static ResultadoRegistro ok(
            int llamadaId,
            BigDecimal saldoAnterior,
            BigDecimal saldoPosterior,
            BigDecimal montoRebajado
        ) {
            return new ResultadoRegistro(
                true,
                "Movimiento registrado correctamente",
                llamadaId,
                saldoAnterior,
                saldoPosterior,
                montoRebajado
            );
        }

        public static ResultadoRegistro error(String mensaje)
        {
            String detalle = mensaje == null || mensaje.isBlank()
                ? "Error registrando movimiento"
                : mensaje;
            return new ResultadoRegistro(false, detalle, 0, BigDecimal.ZERO, BigDecimal.ZERO, BigDecimal.ZERO);
        }

        public boolean isExitoso() { return exitoso; }
        public String getMensaje() { return mensaje; }
        public int getLlamadaId() { return llamadaId; }
        public BigDecimal getSaldoAnterior() { return saldoAnterior; }
        public BigDecimal getSaldoPosterior() { return saldoPosterior; }
        public BigDecimal getMontoRebajado() { return montoRebajado; }
    }
}
