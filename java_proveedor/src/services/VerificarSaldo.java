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
                return "{\"status\":\"ERROR\",\"mensaje\":\"Error SQL Server: " + sanitizar(error) + "\"}";
            }

            return "{\"status\":\"ERROR\",\"mensaje\":\"Linea no registrada en SQL Server\"}";
        }

        if (!servicio.isActivo()) {
            return "{\"status\":\"ERROR\",\"mensaje\":\"Linea inactiva\"}";
        }

        if ("POSTPAGO".equalsIgnoreCase(servicio.getTipoServicio())) {
            return "{\"status\":\"OK\",\"tiempo_maximo_segundos\":-1,\"tipo_servicio\":\"POSTPAGO\"}";
        }

        BigDecimal saldo = servicio.getSaldoDisponible();
        BigDecimal costoMinuto = servicio.getTarifaAplicable();

        if (saldo.compareTo(costoMinuto) < 0 || saldo.compareTo(BigDecimal.ZERO) <= 0) {
            return "{\"status\":\"INSUF\",\"mensaje\":\"Saldo insuficiente para iniciar la llamada\"}";
        }

        BigDecimal minutosDisponibles = saldo.divide(costoMinuto, 2, RoundingMode.DOWN);
        BigDecimal segundosDisponibles = minutosDisponibles.multiply(BigDecimal.valueOf(60));
        int tiempoMaximoSegundos = segundosDisponibles.intValue();

        return "{\"status\":\"OK\",\"tiempo_maximo_segundos\":" + tiempoMaximoSegundos + ",\"tipo_servicio\":\"PREPAGO\"}";
    }

    private String sanitizar(String texto) {
        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
