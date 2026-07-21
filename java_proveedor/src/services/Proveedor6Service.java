package java_proveedor.src.services;

import java_proveedor.src.database.FacturacionDAO;

public class Proveedor6Service {

    private final FacturacionDAO facturacionDAO = new FacturacionDAO();

    public String procesar(String trama) {
        try {
        String fechaCalculo = leerCampo(trama, "fecha_calculo");
        String fechaMaximaPago = leerCampo(trama, "fecha_maxima_pago");

        if (fechaCalculo.isEmpty() || fechaMaximaPago.isEmpty()) {
        return "Datos Incompletos";
        }

        if (!esFechaValida(fechaCalculo) || !esFechaValida(fechaMaximaPago)) {
        return "Datos Incompletos";
        }

        facturacionDAO.calcularFacturacionPostpago(fechaCalculo, fechaMaximaPago);
        return "OK";

        } catch (Exception e) {
        System.err.println("[PROVEEDOR6] Error: " + e.getMessage());
        return "ERROR";
        }
    }

    private boolean esFechaValida(String fecha) {
        if (!fecha.matches("\\d{4}-\\d{2}-\\d{2}")) return false;
            try {
                java.sql.Date.valueOf(fecha);
                return true;
            } catch (IllegalArgumentException e) {
                return false;
            }
        }

    private String leerCampo(String json, String campo) {
        String buscar = "\"" + campo + "\":\"";
        int inicio = json.indexOf(buscar);
        if (inicio == -1) return "";
        inicio += buscar.length();
        int fin = json.indexOf("\"", inicio);
        if (fin == -1) return "";
        return json.substring(inicio, fin).trim();
    }
}
