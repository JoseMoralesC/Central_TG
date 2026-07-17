package java_proveedor.src.services;

import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.database.ServicioDAO.LineaProveedor5;

import java.math.BigDecimal;

public class Proveedor5Service {

    private static final BigDecimal SALDO_INICIAL_PREPAGO = new BigDecimal("1000.00");

    private ServicioDAO servicioDAO = new ServicioDAO();
    private Identificador6Client identificador6Client = new Identificador6Client();

    public String procesar(String solicitud) {
        TramaProveedor5 trama = TramaProveedor5.desdeJson(solicitud);

        if (!trama.esCompleta()) {
            return "Datos Incompletos";
        }

        if (!trama.tipoValido() || !trama.accionValida()) {
            return "ERROR";
        }

        LineaProveedor5 linea = servicioDAO.obtenerLineaProveedor5(trama.telefono);
        if (linea == null) {
            return "ERROR";
        }

        String validacion = validarOperacion(trama, linea);
        if (!"OK".equals(validacion)) {
            return validacion;
        }

        boolean sincronizado = identificador6Client.sincronizar(
            construirTramaIdentificador6(trama)
        );

        if (!sincronizado) {
            return "Activacion fallida";
        }

        boolean actualizado = "ACTIVAR".equals(trama.accion)
            ? servicioDAO.activarLineaProveedor5(
                linea.servicioId,
                trama.identificadorTelefono,
                trama.identificadorTarjeta,
                trama.tipoServicio,
                trama.identificacionDueno,
                "PREPAGO".equals(trama.tipoServicio)
                    ? SALDO_INICIAL_PREPAGO
                    : BigDecimal.ZERO
            )
            : servicioDAO.desactivarLineaProveedor5(linea.servicioId);

        return actualizado ? "OK" : "ERROR";
    }

    private String validarOperacion(TramaProveedor5 trama, LineaProveedor5 linea) {
        if (!coincideSiExiste(linea.identificadorTelefono, trama.identificadorTelefono)
            || !coincideSiExiste(linea.identificadorTarjeta, trama.identificadorTarjeta)
            || !trama.tipoServicio.equalsIgnoreCase(linea.tipoServicio)) {
            return "ERROR";
        }

        if ("ACTIVAR".equals(trama.accion)) {
            if (linea.activo || "ACTIVO".equalsIgnoreCase(linea.estadoLinea)) {
                return "Telefono en uso";
            }

            return "OK";
        }

        if (!linea.activo || !"ACTIVO".equalsIgnoreCase(linea.estadoLinea)) {
            return "Telefono no corresponde";
        }

        if (!trama.identificacionDueno.equals(linea.identificacionDueno)) {
            return "Telefono no corresponde";
        }

        return "OK";
    }

    private boolean coincideSiExiste(String almacenado, String recibido) {
        return almacenado == null || almacenado.isBlank() || almacenado.equals(recibido);
    }

    private String construirTramaIdentificador6(TramaProveedor5 trama) {
        return "{"
            + "\"tipo_transaccion\":\"IDENTIFICADOR6\","
            + "\"telefono\":\"" + sanitizar(trama.telefono) + "\","
            + "\"identificador_dispositivo\":\"" + sanitizar(trama.identificadorTelefono) + "\","
            + "\"identificador_tarjeta\":\"" + sanitizar(trama.identificadorTarjeta) + "\","
            + "\"tipo_servicio\":\"" + sanitizar(trama.tipoServicio) + "\","
            + "\"identificacion_cliente\":\"" + sanitizar(trama.identificacionDueno) + "\","
            + "\"proveedor_codigo\":\"XYZ\","
            + "\"accion\":\"" + sanitizar(trama.accion) + "\","
            + "\"fecha_hora\":\"" + sanitizar(trama.fechaHora) + "\""
        + "}";
    }

    private String sanitizar(String texto) {
        if (texto == null) {
            return "";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }

    private static class TramaProveedor5 {
        String telefono;
        String identificadorTelefono;
        String identificadorTarjeta;
        String tipoServicio;
        String identificacionDueno;
        String accion;
        String fechaHora;

        static TramaProveedor5 desdeJson(String json) {
            TramaProveedor5 trama = new TramaProveedor5();
            trama.telefono = leerCampo(json, "telefono");
            trama.identificadorTelefono = leerCampo(json, "identificador_dispositivo");
            if (trama.identificadorTelefono.isBlank()) {
                trama.identificadorTelefono = leerCampo(json, "identificador_telefono");
            }
            trama.identificadorTarjeta = leerCampo(json, "identificador_tarjeta");
            trama.tipoServicio = leerCampo(json, "tipo_servicio").toUpperCase();
            trama.identificacionDueno = leerCampo(json, "identificacion_dueno");
            if (trama.identificacionDueno.isBlank()) {
                trama.identificacionDueno = leerCampo(json, "identificacion_cliente");
            }
            trama.accion = leerCampo(json, "accion").toUpperCase();
            trama.fechaHora = leerCampo(json, "fecha_hora");
            return trama;
        }

        boolean esCompleta() {
            return !telefono.isBlank()
                && !identificadorTelefono.isBlank()
                && !identificadorTarjeta.isBlank()
                && !tipoServicio.isBlank()
                && !identificacionDueno.isBlank()
                && !accion.isBlank();
        }

        boolean tipoValido() {
            return "PREPAGO".equals(tipoServicio) || "POSTPAGO".equals(tipoServicio);
        }

        boolean accionValida() {
            return "ACTIVAR".equals(accion) || "DESACTIVAR".equals(accion);
        }

        private static String leerCampo(String json, String campo) {
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

                    if (!texto && (actual == ',' || actual == '}')) {
                        break;
                    }

                    fin++;
                }

                return desescaparJson(json.substring(inicio, fin).trim());
            } catch (Exception e) {
                return "";
            }
        }

        private static String desescaparJson(String valor) {
            return valor
                .replace("\\/", "/")
                .replace("\\\"", "\"")
                .replace("\\\\", "\\");
        }
    }
}
