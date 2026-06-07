package java_proveedor.src.models;

import java.time.LocalDateTime;
import java.math.BigDecimal;

public class Servicio {
    private int servicioId;
    private int clienteId;
    private String numeroTelefono;
    private String tipoServicio; // "PREPAGO" o "POSTPAGO"
    private boolean activo;
    private LocalDateTime fechaRegistro;
    private BigDecimal tarifaAplicable; 

    
    // Composición: El saldo forma parte intrínseca de la sesión de la línea telefónica
    private BigDecimal saldoDisponible;
    private LocalDateTime fechaActualizacionSaldo;

    public Servicio() {}

    // Lógica de negocio embebida: Verifica si el servicio está apto para llamadas
    public boolean puedeRealizarLlamadas() {
        if (!this.activo) return false;
        if ("PREPAGO".equalsIgnoreCase(this.tipoServicio)) {
            return this.saldoDisponible != null && this.saldoDisponible.compareTo(BigDecimal.ZERO) > 0;
        }
        return true; // Postpago siempre puede iniciar
    }

    // Getters y Setters
    public int getServicioId() { return servicioId; }
    public void setServicioId(int servicioId) { this.servicioId = servicioId; }

    public int getClienteId() { return clienteId; }
    public void setClienteId(int clienteId) { this.clienteId = clienteId; }

    public String getNumeroTelefono() { return numeroTelefono; }
    public void setNumeroTelefono(String numeroTelefono) { this.numeroTelefono = numeroTelefono; }

    public String getTipoServicio() { return tipoServicio; }
    public void setTipoServicio(String tipoServicio) { this.tipoServicio = tipoServicio; }

    public boolean isActivo() { return activo; }
    public void setActivo(boolean activo) { this.activo = activo; }

    public LocalDateTime getFechaRegistro() { return fechaRegistro; }
    public void setFechaRegistro(LocalDateTime fechaRegistro) { this.fechaRegistro = fechaRegistro; }

    public BigDecimal getSaldoDisponible() { return saldoDisponible; }
    public void setSaldoDisponible(BigDecimal saldoDisponible) { this.saldoDisponible = saldoDisponible; }

    public LocalDateTime getFechaActualizacionSaldo() { return fechaActualizacionSaldo; }
    public void setFechaActualizacionSaldo(LocalDateTime fechaActualizacionSaldo) { this.fechaActualizacionSaldo = fechaActualizacionSaldo; }

    public BigDecimal getTarifaAplicable() { return tarifaAplicable;}

    public void setTarifaAplicable(BigDecimal tarifaAplicable) { this.tarifaAplicable = tarifaAplicable; }
}