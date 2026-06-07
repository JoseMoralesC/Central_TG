package java_proveedor.src.models;

import java.math.BigDecimal;
import java.time.LocalDate;

public class LlamadaProveedor
{
    private int llamadaId;
    private int servicioId; // FK hacia el Servicio que origina
    private int tarifaId;   // FK hacia la Tarifa aplicada
    private String telefonoDestino;
    private LocalDate fechaLlamada;
    private String horaLlamada;
    private BigDecimal costo;
    private String duracion; // Almacenado como formato "HH:MM:SS" o minutos en texto

    public LlamadaProveedor() {}

    // Lógica de cálculo financiero para la Historia de Usuario 3
    public void calcularCostoLlamada(BigDecimal costoPorMinuto, int minutosConsumidos) {
        if (costoPorMinuto != null && minutosConsumidos > 0) {
            this.costo = costoPorMinuto.multiply(new BigDecimal(minutosConsumidos));
        } else {
            this.costo = BigDecimal.ZERO;
        }
    }

    // Getters y Setters
    public int getLlamadaId() { return llamadaId; }
    public void setLlamadaId(int llamadaId) { this.llamadaId = llamadaId; }

    public int getServicioId() { return servicioId; }
    public void setServicioId(int servicioId) { this.servicioId = servicioId; }

    public int getTarifaId() { return tarifaId; }
    public void setTarifaId(int tarifaId) { this.tarifaId = tarifaId; }

    public String getTelefonoDestino() { return telefonoDestino; }
    public void setTelefonoDestino(String telefonoDestino) { this.telefonoDestino = telefonoDestino; }

    public LocalDate getFechaLlamada() { return fechaLlamada; }
    public void setFechaLlamada(LocalDate fechaLlamada) { this.fechaLlamada = fechaLlamada; }

    public String getHoraLlamada() { return horaLlamada; }
    public void setHoraLlamada(String horaLlamada) { this.horaLlamada = horaLlamada; }

    public BigDecimal getCosto() { return costo; }
    public void setCosto(BigDecimal costo) { this.costo = costo; }

    public String getDuracion() { return duracion; }
    public void setDuracion(String duracion) { this.duracion = duracion; }

}