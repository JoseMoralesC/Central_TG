package java_proveedor.src.config;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class Config_Loader {

    //Atributos
    private static Properties properties = new Properties();

    //Bloque de codigo implementado por IA para cargar el archivo de configuración al iniciar la clase
    static {
        //Se busca el archivo en la ruta relativa del proyecto
        try (InputStream input = Config_Loader.class.getResourceAsStream("/java_proveedor/src/config/config.properties")) {
            if (input == null) {
                // Si falla el recurso interno, intentamos buscarlo de manera relativa al directorio de ejecución
                try (InputStream fallbackInput = new java.io.FileInputStream("java_proveedor/src/config/config.properties")) {
                    properties.load(fallbackInput);
                    System.out.println("[Config] Configuración cargada con éxito (Ruta Relativa).");
                } catch (IOException e) {
                    // Último intento: Buscar en la raíz de donde estés parado por si acaso
                    try (InputStream absoluteInput = new java.io.FileInputStream("src/config/config.properties")) {
                        properties.load(absoluteInput);
                        System.out.println("[Config] Configuración cargada con éxito (Ruta Src).");
                    } catch (IOException ex2) {
                        System.err.println("[Error] No se pudo encontrar el archivo config.properties en ninguna ruta.");
                    }
                }
            } else {
                properties.load(input);
                System.out.println("[Config] Configuración cargada con éxito desde Classpath.");
            }
        } catch (IOException e) {
            System.err.println("[Error] No se pudo cargar el archivo config.properties: " + e.getMessage());
        }
    }

    // Método público para obtener cualquier valor pasando su llave
    public static String get(String key)
    {
        return properties.getProperty(key);
    }
}
