package java_proveedor.src.database;

import java_proveedor.src.config.Config_Loader;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;


public class ConexionSQL{

    // Registramos explícitamente el Driver de SQL Server
    static {
        try
        {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
        
        }
        
        catch (ClassNotFoundException e)
        {
            System.err.println("[Error] No se encontró el Driver JDBC de SQL Server. ¿Agregaste la dependencia/JAR?");
            e.printStackTrace();
        }
    }

    /**
     * Obtiene una nueva conexión a la base de datos SQL Server local
     * @return Connection objeto de conexión listo para usar
     * @throws SQLException si las credenciales o la URL están fallando
     */
    public static Connection getConexion() throws SQLException {
        String url = Config_Loader.get("db.url");
        String usuario = Config_Loader.get("db.usuario");
        String contrasena = Config_Loader.get("db.contrasena");

        return DriverManager.getConnection(url, usuario, contrasena);
    }
}