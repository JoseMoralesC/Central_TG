package java_proveedor.src.services;

import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.models.Servicio;
import java.math.BigDecimal;
import java.math.RoundingMode;

public class VerificarSaldo {

    private ServicioDAO servicioDAO = new ServicioDAO();

    /**
     * Lógica de las Fases 4 y 5 del Roadmap: Verifica viabilidad de llamada y calcula el tiempo disponible.
     * @param numeroOrigen Número que realiza la llamada.
     * @param tipoDestino Tipo de destino para extraer la tarifa (LOCAL / INTERNACIONAL).
     * @return Una cadena formateada en JSON con la respuesta para Python (OK, INSUF o ERROR).
     */
    public String procesarVerificacionLlamada(String numeroOrigen, String tipoDestino) {
        // 1. Ir a la capa DAL por los datos cruzados de SQL Server
        Servicio servicio = servicioDAO.obtenerDetalleParaLlamada(numeroOrigen, tipoDestino);

        // --- FASE 4: VALIDACIÓN DE ESTADO ---
        if (servicio == null || !servicio.isActivo()) {
            return "{\"status\": \"ERROR\", \"mensaje\": \"Línea inactiva o no registrada\"}";
        }

        // Si es POSTPAGO, tiene llamadas ilimitadas por contrato, pasa directo sin restricción de tiempo
        if ("POSTPAGO".equalsIgnoreCase(servicio.getTipoServicio())) {
            return "{\"status\": \"OK\", \"tiempo_maximo_segundos\": -1, \"tipo_servicio\": \"POSTPAGO\"}";
        }

        // --- FASE 5: CÁLCULO DE TARIFA Y TIEMPO MÁXIMO (PREPAGO) ---
        BigDecimal saldo = servicio.getSaldoDisponible();
        BigDecimal costoMinuto = servicio.getTarifaAplicable();

        // Regla estricta: Validar si tiene saldo suficiente para al menos 1 minuto (60 segundos)
        if (saldo.compareTo(costoMinuto) < 0 || saldo.compareTo(BigDecimal.ZERO) <= 0) {
            return "{\"status\": \"INSUF\", \"mensaje\": \"Saldo insuficiente para iniciar la llamada\"}";
        }

        // Calcular tiempo máximo: (Saldo / Costo por Minuto) * 60 segundos
        // Usamos RoundingMode.DOWN para no redondear hacia arriba regalando segundos que no puede pagar
        BigDecimal minutosDisponibles = saldo.divide(costoMinuto, 2, RoundingMode.DOWN);
        BigDecimal segundosDisponibles = minutosDisponibles.multiply(BigDecimal.valueOf(60));
        int tiempoMaximoSegundos = segundosDisponibles.intValue();

        return "{\"status\": \"OK\", \"tiempo_maximo_segundos\": " + tiempoMaximoSegundos + ", \"tipo_servicio\": \"PREPAGO\"}";
    }
}