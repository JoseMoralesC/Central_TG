package java_proveedor.src.config;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class ConfigLoader {
    private static Properties properties = new Properties();

    // Este bloque estático se ejecuta automáticamente al usar la clase
    static {
        // Buscamos el archivo en la ruta relativa del proyecto
        try (InputStream input = new FileInputStream("src/config/config.properties")) {
            // Cargamos las propiedades en memoria
            properties.load(input);
            System.out.println("[Config] Configuración cargada con éxito.");
        } catch (IOException ex) {
            System.err.println("[Error] No se pudo cargar el archivo config.properties: " + ex.getMessage());
        }
    }

    // Método público para obtener cualquier valor pasando su llave
    public static String get(String key) {
        return properties.getProperty(key);
    }
}
