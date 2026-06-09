package java_proveedor.src.database;

import java_proveedor.src.models.Tarifa;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

public class TarifaDAO
{
    private String ultimoError = "";

    public TarifaDAO() {}

    public String getUltimoError()
    {
        return ultimoError;
    }

    public Tarifa obtenerTarifaActivaPorTipo(String tipoLlamada)
    {
        ultimoError = "";

        String sql =
            "SELECT tarifa_id, tipo_llamada, descripcion, costo_por_minuto, activa " +
            "FROM tarifas " +
            "WHERE tipo_llamada = ? AND activa = 1";

        try (
            Connection conn = ConexionSQL.getConexion();
            PreparedStatement ps = conn.prepareStatement(sql)
        ) {
            ps.setString(1, normalizarTipoLlamada(tipoLlamada));

            try (ResultSet rs = ps.executeQuery()) {
                if (rs.next()) {
                    Tarifa tarifa = new Tarifa();
                    tarifa.setTarifaId(rs.getInt("tarifa_id"));
                    tarifa.setTipoLlamada(rs.getString("tipo_llamada"));
                    tarifa.setDescripcion(rs.getString("descripcion"));
                    tarifa.setCostoPorMinuto(rs.getBigDecimal("costo_por_minuto"));
                    tarifa.setActiva(rs.getBoolean("activa"));
                    return tarifa;
                }
            }
        } catch (Exception e) {
            ultimoError = e.getMessage();
            System.err.println("[Error TarifaDAO] No se pudo consultar tarifa: " + tipoLlamada);
            e.printStackTrace();
        }

        return null;
    }

    private String normalizarTipoLlamada(String tipoLlamada)
    {
        if (tipoLlamada == null || tipoLlamada.isBlank()) {
            return "NACIONAL";
        }

        return tipoLlamada.trim().toUpperCase();
    }
}
