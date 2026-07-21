package java_proveedor.src.services;

import java_proveedor.src.config.Config_Loader;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

public class Identificador6Client {

    public boolean sincronizar(String tramaJson) {
        String host = valorPorDefecto(
            Config_Loader.get("identificador.host"),
            "127.0.0.1"
        );
        int puerto = leerEntero(
            Config_Loader.get("identificador.puerto"),
            5000
        );

        try (
            Socket socket = new Socket();
        ) {
            socket.connect(new InetSocketAddress(host, puerto), 3000);
            socket.setSoTimeout(5000);

            try (
            PrintWriter writer = new PrintWriter(
                new OutputStreamWriter(socket.getOutputStream(), StandardCharsets.UTF_8),
                true
            );
            BufferedReader reader = new BufferedReader(
                new InputStreamReader(socket.getInputStream(), StandardCharsets.UTF_8)
            )
        ) {
            writer.println(tramaJson);
            String respuesta = reader.readLine();

            return respuesta != null
                && respuesta.replace(" ", "").contains("\"codigo\":\"OK\"");
            }
        } catch (Exception e) {
            System.err.println("[Identificador6Client] Error al sincronizar: " + e.getMessage());
            return false;
        }
    }

    private String valorPorDefecto(String valor, String defecto) {
        return valor == null || valor.isBlank() ? defecto : valor.trim();
    }

    private int leerEntero(String valor, int defecto) {
        try {
            return Integer.parseInt(valor);
        } catch (Exception e) {
            return defecto;
        }
    }
}
