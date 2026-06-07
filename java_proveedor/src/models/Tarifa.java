package java_proveedor.src.models;

import java.math.BigDecimal;

public class Tarifa
{
    private int tarifaId;
    private String tipoLlamada; // Ej: "NACIONAL", "INTERNACIONAL"
    private String descripcion;
    private BigDecimal costoPorMinuto;
    private boolean activa;

    public Tarifa() {}

    public int getTarifaId() { return tarifaId; }
    public void setTarifaId(int tarifaId) { this.tarifaId = tarifaId; }

    public String getTipoLlamada() { return tipoLlamada; }
    public void setTipoLlamada(String tipoLlamada) { this.tipoLlamada = tipoLlamada; }

    public String getDescripcion() { return descripcion; }
    public void setDescripcion(String descripcion) { this.descripcion = descripcion; }

    public BigDecimal getCostoPorMinuto() { return costoPorMinuto; }
    public void setCostoPorMinuto(BigDecimal costoPorMinuto) { this.costoPorMinuto = costoPorMinuto; }

    public boolean isActiva() { return activa; }
    public void setActiva(boolean activa) { this.activa = activa; }
    
}
