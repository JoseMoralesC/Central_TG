package java_proveedor.src.services;

import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.models.Servicio;
import java.math.BigDecimal;
import java.math.RoundingMode;

public class VerificarSaldo {

    private ServicioDAO servicioDAO = new ServicioDAO();

    public String procesarVerificacionLlamada(String numeroOrigen, String tipoDestino) {
        Servicio servicio = servicioDAO.obtenerDetalleParaLlamada(numeroOrigen, tipoDestino);

        if (servicio == null) {
            String error = servicioDAO.getUltimoError();
            if (error != null && !error.isBlank()) {
                return respuestaError("Error SQL Server: " + sanitizar(error));
            }

            return respuestaError("Linea no registrada en SQL Server");
        }

        if (!servicio.isActivo()) {
            return respuestaError("Linea inactiva");
        }

        if ("POSTPAGO".equalsIgnoreCase(servicio.getTipoServicio())) {
            return respuestaOk(
                "POSTPAGO",
                BigDecimal.valueOf(-1),
                servicio.getTarifaAplicable(),
                -1
            );
        }

        BigDecimal saldo = servicio.getSaldoDisponible();
        BigDecimal costoMinuto = servicio.getTarifaAplicable();

        if (saldo.compareTo(costoMinuto) < 0 || saldo.compareTo(BigDecimal.ZERO) <= 0) {
            return respuestaInsuficiente("Saldo insuficiente para iniciar la llamada");
        }

        BigDecimal minutosDisponibles = saldo.divide(costoMinuto, 2, RoundingMode.DOWN);
        BigDecimal segundosDisponibles = minutosDisponibles.multiply(BigDecimal.valueOf(60));
        int tiempoMaximoSegundos = segundosDisponibles.intValue();

        return respuestaOk("PREPAGO", saldo, costoMinuto, tiempoMaximoSegundos);
    }

    private String respuestaOk(
        String tipoServicio,
        BigDecimal saldoDisponible,
        BigDecimal costoMinuto,
        int tiempoMaximoSegundos
    ) {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"VERIFICAR_SALDO\","
            + "\"status\":\"OK\","
            + "\"estado\":\"OK\","
            + "\"tiempo_maximo_segundos\":" + tiempoMaximoSegundos + ","
            + "\"tipo_servicio\":\"" + tipoServicio + "\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"AUTORIZADO\","
                + "\"mensaje\":\"Servicio autorizado por el proveedor\""
            + "},"
            + "\"datos_autorizacion\":{"
                + "\"tipo_servicio\":\"" + tipoServicio + "\","
                + "\"saldo_disponible\":" + decimal(saldoDisponible) + ","
                + "\"costo_por_minuto\":" + decimal(costoMinuto) + ","
                + "\"tiempo_maximo_segundos\":" + tiempoMaximoSegundos + ","
                + "\"moneda\":\"CRC\""
            + "}"
        + "}";
    }

    private String respuestaInsuficiente(String mensaje)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"VERIFICAR_SALDO\","
            + "\"status\":\"INSUF\","
            + "\"estado\":\"INSUF\","
            + "\"mensaje\":\"" + sanitizar(mensaje) + "\","
            + "\"resultado\":{"
                + "\"codigo\":\"INSUF\","
                + "\"estado\":\"SALDO_INSUFICIENTE\","
                + "\"mensaje\":\"" + sanitizar(mensaje) + "\""
            + "}"
        + "}";
    }

    private String respuestaError(String mensaje)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"VERIFICAR_SALDO\","
            + "\"status\":\"ERROR\","
            + "\"estado\":\"ERROR\","
            + "\"mensaje\":\"" + sanitizar(mensaje) + "\","
            + "\"resultado\":{"
                + "\"codigo\":\"ERROR\","
                + "\"estado\":\"ERROR\","
                + "\"mensaje\":\"" + sanitizar(mensaje) + "\""
            + "}"
        + "}";
    }

    private String decimal(BigDecimal valor)
    {
        BigDecimal seguro = valor != null ? valor : BigDecimal.ZERO;
        return seguro.setScale(2, RoundingMode.HALF_UP).toPlainString();
    }

    private String sanitizar(String texto) {
        if (texto == null || texto.isBlank()) {
            return "Error no especificado";
        }
        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
