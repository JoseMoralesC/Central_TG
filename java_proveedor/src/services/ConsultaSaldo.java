package java_proveedor.src.services;

import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.models.Servicio;

public class ConsultaSaldo {

    private ServicioDAO servicioDAO = new ServicioDAO();
    private String ultimoError = "";

    public String getUltimoError() {
        return ultimoError;
    }

    public String procesarConsulta(String numeroTelefono) {
        ultimoError = "";

        if (numeroTelefono == null || numeroTelefono.trim().isEmpty()) {
            ultimoError = "Numero de telefono vacio";
            return "ERROR";
        }

        Servicio servicio = servicioDAO.obtenerDetalleParaLlamada(numeroTelefono, "NACIONAL");

        if (servicio == null) {
            String error = servicioDAO.getUltimoError();
            ultimoError = error != null && !error.isBlank()
                ? "Error SQL Server: " + error
                : "Linea no registrada en SQL Server";
            return "ERROR";
        }

        if (!servicio.isActivo()) {
            ultimoError = "Linea inactiva";
            return "ERROR";
        }

        if ("POSTPAGO".equalsIgnoreCase(servicio.getTipoServicio())) {
            return "-1";
        }

        return servicio.getSaldoDisponible().toString();
    }
}
