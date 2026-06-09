package java_proveedor.src.services;

import java.math.BigDecimal;
import java_proveedor.src.database.ServicioDAO;

public class AdministracionTelefonica {

    private ServicioDAO servicioDAO = new ServicioDAO();

    public String procesarDetalleTelefono(String numeroTelefono) {
        if (numeroTelefono == null || numeroTelefono.isBlank()) {
            return respuestaError("DETALLE_TELEFONO", "Numero de telefono obligatorio");
        }

        String detalle = servicioDAO.obtenerDetalleAdministrativoJson(numeroTelefono);
        if (detalle == null || detalle.isBlank()) {
            String error = servicioDAO.getUltimoError();
            return respuestaError(
                "DETALLE_TELEFONO",
                error != null && !error.isBlank() ? error : "Telefono no encontrado"
            );
        }

        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"DETALLE_TELEFONO\","
            + "\"status\":\"OK\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"CONSULTA_EXITOSA\","
                + "\"mensaje\":\"Detalle consultado correctamente\""
            + "},"
            + "\"telefono\":" + detalle
        + "}";
    }

    public String procesarRecarga(String numeroTelefono, String montoTexto) {
        if (numeroTelefono == null || numeroTelefono.isBlank()) {
            return respuestaError("RECARGAR_SALDO", "Numero de telefono obligatorio");
        }

        BigDecimal monto = leerDecimal(montoTexto);
        if (monto.compareTo(BigDecimal.ZERO) <= 0) {
            return respuestaError("RECARGAR_SALDO", "Monto de recarga invalido");
        }

        boolean ok = servicioDAO.recargarSaldo(numeroTelefono, monto);
        if (!ok) {
            String error = servicioDAO.getUltimoError();
            return respuestaError(
                "RECARGAR_SALDO",
                error != null && !error.isBlank() ? error : "No se pudo recargar saldo"
            );
        }

        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"RECARGAR_SALDO\","
            + "\"status\":\"OK\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"RECARGA_EXITOSA\","
                + "\"mensaje\":\"Saldo recargado correctamente\""
            + "}"
        + "}";
    }

    public String procesarRegistro(
        String numeroTelefono,
        String tipoServicio,
        String proveedorCodigo,
        String saldoInicialTexto,
        boolean activo
    ) {
        if (numeroTelefono == null || numeroTelefono.isBlank()) {
            return respuestaError("REGISTRAR_TELEFONO", "Numero de telefono obligatorio");
        }

        BigDecimal saldoInicial = leerDecimal(saldoInicialTexto);
        boolean ok = servicioDAO.registrarTelefono(
            numeroTelefono,
            valorPorDefecto(tipoServicio, "PREPAGO").toUpperCase(),
            valorPorDefecto(proveedorCodigo, "KOLBI").toUpperCase(),
            saldoInicial,
            activo
        );

        if (!ok) {
            String error = servicioDAO.getUltimoError();
            return respuestaError(
                "REGISTRAR_TELEFONO",
                error != null && !error.isBlank() ? error : "No se pudo registrar telefono"
            );
        }

        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"REGISTRAR_TELEFONO\","
            + "\"status\":\"OK\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"REGISTRO_EXITOSO\","
                + "\"mensaje\":\"Telefono registrado en proveedor\""
            + "}"
        + "}";
    }

    public String procesarCambioEstado(String numeroTelefono, boolean activo) {
        if (numeroTelefono == null || numeroTelefono.isBlank()) {
            return respuestaError("CAMBIAR_ESTADO_TELEFONO", "Numero de telefono obligatorio");
        }

        boolean ok = servicioDAO.cambiarEstadoTelefono(numeroTelefono, activo);
        if (!ok) {
            String error = servicioDAO.getUltimoError();
            return respuestaError(
                "CAMBIAR_ESTADO_TELEFONO",
                error != null && !error.isBlank() ? error : "No se pudo actualizar estado"
            );
        }

        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"CAMBIAR_ESTADO_TELEFONO\","
            + "\"status\":\"OK\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"ESTADO_ACTUALIZADO\","
                + "\"mensaje\":\"Estado actualizado correctamente\""
            + "}"
        + "}";
    }

    private String respuestaError(String accion, String mensaje) {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"" + sanitizar(accion) + "\","
            + "\"status\":\"ERROR\","
            + "\"resultado\":{"
                + "\"codigo\":\"ERROR\","
                + "\"estado\":\"ERROR\","
                + "\"mensaje\":\"" + sanitizar(mensaje) + "\""
            + "}"
        + "}";
    }

    private BigDecimal leerDecimal(String texto) {
        try {
            return new BigDecimal(texto == null || texto.isBlank() ? "0" : texto);
        } catch (Exception e) {
            return BigDecimal.ZERO;
        }
    }

    private String valorPorDefecto(String valor, String defecto) {
        return valor == null || valor.isBlank() ? defecto : valor;
    }

    private String sanitizar(String texto) {
        if (texto == null) {
            return "";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
