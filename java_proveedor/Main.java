package java_proveedor;

import java_proveedor.src.database.ConexionSQL;
import java.sql.Connection;

public class Main
{
    public static void main(String[] args) {
        System.out.println("=== Iniciando Servidor Proveedor ===");
        
        // Forzamos el inicio de la conexión de prueba
        try (Connection conn = ConexionSQL.getConexion())
        {
            if (conn != null && !conn.isClosed()) {
                System.out.println("[Éxito] ¡Conexión establecida con SQL Server Local!");
            }
        }
        
        catch (Exception e)
        {
            System.err.println("[Fallo] Error al conectar a la Base de Datos:");
            e.printStackTrace();
        }
    }
}