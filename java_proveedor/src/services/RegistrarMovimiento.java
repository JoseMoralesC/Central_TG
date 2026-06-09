package java_proveedor.src.services;

import java_proveedor.src.database.LlamadaProveedorDAO;
import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.database.TarifaDAO;
import java_proveedor.src.models.Servicio;
import java_proveedor.src.models.Tarifa;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDateTime;

public class RegistrarMovimiento
{
    private final ServicioDAO servicioDAO = new ServicioDAO();
    private final TarifaDAO tarifaDAO = new TarifaDAO();
    private final LlamadaProveedorDAO llamadaProveedorDAO = new LlamadaProveedorDAO();

    public RegistrarMovimiento()
    {
        
    }

    public String procesarRegistroMovimiento(String solicitudJson)
    {
        try {
            MovimientoRequest request = MovimientoRequest.desdeJson(solicitudJson);
            String errorValidacion = request.validar();

            if (!errorValidacion.isBlank()) {
                return respuestaError(errorValidacion);
            }

            Servicio servicio = servicioDAO.obtenerDetalleParaLlamada(
                request.telefonoOrigen,
                request.tipoLlamada
            );

            if (servicio == null) {
                String error = servicioDAO.getUltimoError();
                return respuestaError(
                    error != null && !error.isBlank()
                        ? "Error SQL Server: " + error
                        : "Linea no registrada en SQL Server"
                );
            }

            if (!servicio.isActivo()) {
                return respuestaError("Linea inactiva");
            }

            Tarifa tarifa = tarifaDAO.obtenerTarifaActivaPorTipo(request.tipoLlamada);

            if (tarifa == null) {
                String error = tarifaDAO.getUltimoError();
                return respuestaError(
                    error != null && !error.isBlank()
                        ? "Error consultando tarifa: " + error
                        : "Tarifa activa no encontrada para tipo de llamada " + request.tipoLlamada
                );
            }

            BigDecimal montoTotal = request.montoTotal;

            if (montoTotal.compareTo(BigDecimal.ZERO) <= 0) {
                montoTotal = calcularMontoDesdeTarifa(tarifa, request.duracionMinutos);
            }

            LlamadaProveedorDAO.ResultadoRegistro resultado =
                llamadaProveedorDAO.registrarMovimiento(
                    servicio,
                    tarifa,
                    request.telefonoDestino,
                    request.fechaInicio,
                    request.fechaFin,
                    request.duracionSegundos,
                    request.duracionMinutos,
                    request.motivoFinalizacion,
                    montoTotal,
                    request.moneda
                );

            if (!resultado.isExitoso()) {
                return respuestaError(resultado.getMensaje());
            }

            return respuestaOk(resultado);
        } catch (Exception e) {
            return respuestaError(e.getMessage());
        }
    }

    private BigDecimal calcularMontoDesdeTarifa(Tarifa tarifa, int duracionMinutos)
    {
        int minutosFacturados = Math.max(1, duracionMinutos);
        return tarifa.getCostoPorMinuto()
            .multiply(BigDecimal.valueOf(minutosFacturados))
            .setScale(2, RoundingMode.HALF_UP);
    }

    private String respuestaOk(LlamadaProveedorDAO.ResultadoRegistro resultado)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"REGISTRO_MOVIMIENTO\","
            + "\"status\":\"OK\","
            + "\"estado\":\"OK\","
            + "\"mensaje\":\"Movimiento registrado correctamente\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"MOVIMIENTO_REGISTRADO\","
                + "\"mensaje\":\"Movimiento registrado correctamente\""
            + "},"
            + "\"datos_autorizacion\":{"
                + "\"llamada_id\":" + resultado.getLlamadaId() + ","
                + "\"saldo_anterior\":" + decimal(resultado.getSaldoAnterior()) + ","
                + "\"monto_rebajado\":" + decimal(resultado.getMontoRebajado()) + ","
                + "\"saldo_actual\":" + decimal(resultado.getSaldoPosterior()) + ","
                + "\"moneda\":\"CRC\""
            + "}"
        + "}";
    }

    private String respuestaError(String mensaje)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"REGISTRO_MOVIMIENTO\","
            + "\"status\":\"ERROR\","
            + "\"estado\":\"ERROR\","
            + "\"mensaje\":\"" + sanitizar(mensaje) + "\","
            + "\"resultado\":{"
                + "\"codigo\":\"ERROR\","
                + "\"estado\":\"MOVIMIENTO_FALLIDO\","
                + "\"mensaje\":\"" + sanitizar(mensaje) + "\""
            + "}"
        + "}";
    }

    private String decimal(BigDecimal valor)
    {
        BigDecimal seguro = valor != null ? valor : BigDecimal.ZERO;
        return seguro.setScale(2, RoundingMode.HALF_UP).toPlainString();
    }

    private String sanitizar(String texto)
    {
        if (texto == null || texto.isBlank()) {
            return "Error registrando movimiento";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }

    private static class MovimientoRequest
    {
        private String telefonoOrigen;
        private String telefonoDestino;
        private String tipoLlamada;
        private String motivoFinalizacion;
        private String moneda;
        private LocalDateTime fechaInicio;
        private LocalDateTime fechaFin;
        private int duracionSegundos;
        private int duracionMinutos;
        private BigDecimal montoTotal;

        static MovimientoRequest desdeJson(String json)
        {
            MovimientoRequest request = new MovimientoRequest();
            request.telefonoOrigen = leerTexto(json, "telefono_origen");
            request.telefonoDestino = leerTexto(json, "telefono_destino");
            request.tipoLlamada = valorPorDefecto(leerTexto(json, "tipo_llamada"), "NACIONAL");
            request.motivoFinalizacion = valorPorDefecto(
                leerTexto(json, "motivo_finalizacion"),
                "FINALIZACION"
            );
            request.moneda = valorPorDefecto(leerTexto(json, "moneda"), "CRC");
            request.duracionSegundos = leerEntero(json, "duracion_segundos", 0);
            request.duracionMinutos = leerEntero(
                json,
                "duracion_minutos",
                (int)Math.ceil(Math.max(1, request.duracionSegundos) / 60.0)
            );
            request.montoTotal = leerDecimal(json, "monto_total", BigDecimal.ZERO);

            LocalDateTime ahora = LocalDateTime.now();
            request.fechaInicio = leerFecha(json, "fecha_inicio", ahora);
            request.fechaFin = leerFecha(json, "fecha_fin", ahora);

            return request;
        }

        String validar()
        {
            if (telefonoOrigen == null || telefonoOrigen.isBlank()) {
                return "telefono_origen es obligatorio";
            }

            if (telefonoDestino == null || telefonoDestino.isBlank()) {
                return "telefono_destino es obligatorio";
            }

            if (duracionSegundos < 0 || duracionMinutos < 0) {
                return "La duracion no puede ser negativa";
            }

            if (montoTotal.compareTo(BigDecimal.ZERO) < 0) {
                return "El monto no puede ser negativo";
            }

            return "";
        }

        private static String leerTexto(String json, String campo)
        {
            String patron = "\"" + campo + "\"";
            int posClave = json.indexOf(patron);

            if (posClave == -1) {
                return "";
            }

            int posDosPuntos = json.indexOf(":", posClave);
            int posAbreComillas = json.indexOf("\"", posDosPuntos);
            int posCierraComillas = json.indexOf("\"", posAbreComillas + 1);

            if (posDosPuntos == -1 || posAbreComillas == -1 || posCierraComillas == -1) {
                return "";
            }

            return json.substring(posAbreComillas + 1, posCierraComillas);
        }

        private static int leerEntero(String json, String campo, int valorDefecto)
        {
            String valor = leerValorPrimitivo(json, campo);

            if (valor.isBlank()) {
                return valorDefecto;
            }

            try {
                return Integer.parseInt(valor);
            } catch (Exception e) {
                return valorDefecto;
            }
        }

        private static BigDecimal leerDecimal(String json, String campo, BigDecimal valorDefecto)
        {
            String valor = leerValorPrimitivo(json, campo);

            if (valor.isBlank()) {
                return valorDefecto;
            }

            try {
                return new BigDecimal(valor).setScale(2, RoundingMode.HALF_UP);
            } catch (Exception e) {
                return valorDefecto;
            }
        }

        private static String leerValorPrimitivo(String json, String campo)
        {
            String patron = "\"" + campo + "\"";
            int posClave = json.indexOf(patron);

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

            boolean entreComillas = inicio < json.length() && json.charAt(inicio) == '"';

            if (entreComillas) {
                inicio++;
            }

            int fin = inicio;

            while (fin < json.length()) {
                char actual = json.charAt(fin);

                if (entreComillas && actual == '"') {
                    break;
                }

                if (!entreComillas && (actual == ',' || actual == '}' || Character.isWhitespace(actual))) {
                    break;
                }

                fin++;
            }

            return json.substring(inicio, fin).trim();
        }

        private static LocalDateTime leerFecha(
            String json,
            String campo,
            LocalDateTime valorDefecto
        ) {
            String valor = leerTexto(json, campo);

            if (valor.isBlank()) {
                valor = leerTexto(json, "fecha_hora");
            }

            if (valor.isBlank()) {
                return valorDefecto;
            }

            try {
                return LocalDateTime.parse(valor);
            } catch (Exception e) {
                return valorDefecto;
            }
        }

        private static String valorPorDefecto(String valor, String valorDefecto)
        {
            if (valor == null || valor.isBlank()) {
                return valorDefecto;
            }

            return valor.trim().toUpperCase();
        }
    }
}
