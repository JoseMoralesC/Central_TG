package java_proveedor.src.services;

import java_proveedor.src.database.ServicioDAO;
import java_proveedor.src.models.Servicio;

public class ConsultaSaldo {

    private ServicioDAO servicioDAO = new ServicioDAO();

    /**
     * Procesa la lógica de negocio para la consulta de saldo (HU1 / Fases 4 y 5 del Roadmap)
     * @param numeroTelefono Número telefónico a consultar.
     * @return El saldo en texto, "-1" si es postpago, o "ERROR" si la línea no es válida.
     */
    public String procesarConsulta(String numeroTelefono) {
        // 1. Validación básica de entrada
        if (numeroTelefono == null || numeroTelefono.trim().isEmpty()) {
            return "ERROR";
        }

        // 2. Consultar la capa de datos (DAO)
        Servicio servicio = servicioDAO.obtenerDetalleParaLlamada(numeroTelefono, "LOCAL");

        // 3. Aplicar reglas de negocio del Proveedor Telefónico
        if (servicio == null || !servicio.isActivo()) {
            return "ERROR"; // Línea inactiva o inexistente
        }

        // Regla del caso de estudio: "Consulta de saldo postpago retorna -1"
        if ("POSTPAGO".equalsIgnoreCase(servicio.getTipoServicio())) {
            return "-1";
        }

        // Caso Prepago: Retorna el saldo disponible puro en formato String
        return servicio.getSaldoDisponible().toString();
    }
}
