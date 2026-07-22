package java_proveedor.src.services;

import java_proveedor.src.database.FacturacionDAO;
import java.time.LocalDate;
import java.time.format.DateTimeParseException;

public class Proveedor6Service {

    private final FacturacionDAO facturacionDAO = new FacturacionDAO();

    public String procesar(String trama) {
        try {
            String fechaCalculo = leerCampo(trama, "fecha_calculo");
            String fechaMaximaPago = leerCampo(trama, "fecha_maxima_pago");

            if (fechaCalculo.isEmpty() || fechaMaximaPago.isEmpty()) {
                return "ERROR";
            }

            LocalDate fechaCalculoDate = parsearFecha(fechaCalculo);
            LocalDate fechaMaximaPagoDate = parsearFecha(fechaMaximaPago);

            if (fechaCalculoDate == null || fechaMaximaPagoDate == null) {
                return "ERROR";
            }

            if (fechaMaximaPagoDate.isBefore(fechaCalculoDate)) {
                return "ERROR";
            }

            facturacionDAO.calcularFacturacionPostpago(fechaCalculo, fechaMaximaPago);
            return "OK";

        } catch (Exception e) {
            System.err.println("[PROVEEDOR6] Error: " + e.getMessage());
            return "ERROR";
        }
    }

    private LocalDate parsearFecha(String fecha) {
        if (!fecha.matches("\\d{4}-\\d{2}-\\d{2}")) {
            return null;
        }

        try {
            return LocalDate.parse(fecha);
        } catch (DateTimeParseException e) {
            return null;
        }
    }

    private String leerCampo(String json, String campo) {
        try {
            int posClave = json.indexOf("\"" + campo + "\"");

            if (posClave == -1) {
                return "";
            }

            int posDosPuntos = json.indexOf(":", posClave);

            if (posDosPuntos == -1) {
                return "";
            }

            int inicio = posDosPuntos + 1;

            while (inicio < json.length() && Character.isWhitespace(json.charAt(inicio))) {
                inicio++;
            }

            boolean texto = inicio < json.length() && json.charAt(inicio) == '"';

            if (texto) {
                inicio++;
            }

            int fin = inicio;

            while (fin < json.length()) {
                char actual = json.charAt(fin);

                if (texto && actual == '"') {
                    break;
                }

                if (!texto && (actual == ',' || actual == '}' || Character.isWhitespace(actual))) {
                    break;
                }

                fin++;
            }

            return json.substring(inicio, fin).trim();
        } catch (Exception e) {
            return "";
        }
    }
}
