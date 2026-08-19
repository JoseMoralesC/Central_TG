package java_proveedor.src.database;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.Date;

public class FacturacionDAO
{
    public void calcularFacturacionPostpago(String fechaCalculo,
                                            String fechaMaximaPago,
                                            String numeroTelefono)
                                            throws Exception
    {
        String sql = "{call dbo.sp_CalcularFacturacionPostpago(?, ?, ?)}";
        try (Connection conn = ConexionSQL.getConexion();
                CallableStatement cs = conn.prepareCall(sql))
            {
                cs.setDate(1, Date.valueOf(fechaCalculo), null);
                cs.setDate(2, Date.valueOf(fechaMaximaPago), null);
                cs.setString(3, numeroTelefono == null ? "" : numeroTelefono.trim());
                cs.execute();
            }
    }
    
}
