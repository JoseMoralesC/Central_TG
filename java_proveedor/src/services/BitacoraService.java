package java_proveedor.src.services;

import java.io.BufferedWriter;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardOpenOption;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

public class BitacoraService
{
    private static final BitacoraService INSTANCIA = new BitacoraService();
    private static final String RUTA_BITACORA = "logs/proveedor_bitacora.txt";

    private final BlockingQueue<String> colaEventos = new LinkedBlockingQueue<>();

    private BitacoraService()
    {
        Thread hiloBitacora = new Thread(this::procesarCola, "proveedor-bitacora");
        hiloBitacora.setDaemon(true);
        hiloBitacora.start();
    }

    public static BitacoraService obtenerInstancia()
    {
        return INSTANCIA;
    }

    public void registrarEntrada(String trama)
    {
        registrar("ENTRADA", trama);
    }

    public void registrarSalida(String trama)
    {
        registrar("SALIDA", trama);
    }

    public void registrarError(String mensaje)
    {
        String errorJson = "{\"mensaje\":\"" + escapar(mensaje) + "\"}";
        registrar("ERROR", errorJson);
    }

    private void registrar(String tipo, String trama)
    {
        String evento = "{"
            + "\"fecha\":\"" + LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME) + "\","
            + "\"tipo\":\"" + escapar(tipo) + "\","
            + "\"trama\":" + tramaComoJson(trama)
        + "}";

        colaEventos.offer(evento);
    }

    private void procesarCola()
    {
        while (true) {
            try {
                String evento = colaEventos.take();
                escribirEvento(evento);
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                break;
            } catch (Exception e) {
                System.err.println("[Bitacora Proveedor] Error escribiendo bitacora: " + e.getMessage());
            }
        }
    }

    private void escribirEvento(String evento) throws Exception
    {
        Path ruta = Paths.get(RUTA_BITACORA);
        Path carpeta = ruta.getParent();

        if (carpeta != null) {
            Files.createDirectories(carpeta);
        }

        try (BufferedWriter writer = Files.newBufferedWriter(
            ruta,
            StandardCharsets.UTF_8,
            StandardOpenOption.CREATE,
            StandardOpenOption.APPEND
        )) {
            writer.write(evento);
            writer.newLine();
        }
    }

    private String tramaComoJson(String trama)
    {
        if (trama == null || trama.isBlank()) {
            return "{}";
        }

        String limpia = trama.trim();

        if ((limpia.startsWith("{") && limpia.endsWith("}"))
            || (limpia.startsWith("[") && limpia.endsWith("]"))) {
            return limpia;
        }

        return "\"" + escapar(limpia) + "\"";
    }

    private String escapar(String texto)
    {
        if (texto == null) {
            return "";
        }

        return texto
            .replace("\\", "\\\\")
            .replace("\"", "\\\"")
            .replace("\r", "\\r")
            .replace("\n", "\\n");
    }
}
