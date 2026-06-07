package java_proveedor.src.database;

import java_proveedor.src.models.Servicio;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.math.BigDecimal;

public class ServicioDAO {

    public Servicio obtenerDetalleParaLlamada(String numeroTelefono, String tipoDestino) {
        // Query que une el servicio, su saldo y busca la tarifa correspondiente (ej: LOCAL, INTERNACIONAL)
        String sql = "SELECT s.servicio_id, s.cliente_id, s.numero_telefono, s.tipo_servicio, s.activo, " +
                     "sa.saldo_disponible, " +
                     "t.costo_por_minuto " +
                     "FROM servicios s " +
                     "LEFT JOIN saldos sa ON s.servicio_id = sa.servicio_id " +
                     "LEFT JOIN tarifas t ON t.tipo_llamada = ? " + 
                     "WHERE s.numero_telefono = ?";

        try (Connection conn = ConexionSQL.getConexion();
             PreparedStatement ps = conn.prepareStatement(sql)) {
            
            ps.setString(1, tipoDestino); // Ej: "LOCAL" o "INTERNACIONAL"
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
                    
                    // Guardamos la tarifa recuperada temporalmente en el objeto (puedes añadir esta propiedad decimal en tu modelo)
                    BigDecimal tarifa = rs.getBigDecimal("costo_por_minuto");
                    // Asignamos una tarifa por defecto si no se encuentra en la base de datos para evitar fallos catastróficos
                    servicio.setTarifaAplicable(tarifa != null ? tarifa : BigDecimal.valueOf(10.0)); 
                    
                    return servicio;
                }
            }
        } catch (Exception e) {
            System.err.println("[Error ServicioDAO] Error al verificar parámetros de llamada para: " + numeroTelefono);
            e.printStackTrace();
        }
        return null;
    }
}