package java_proveedor.src.models;

import java.util.List;

public class Cliente
{
    private int clienteId;
    private String nombre;
    private String identificacion;
    private String correo;
    private boolean activo;
    private List<Servicio> servicios;

    public Cliente() {}

    public Cliente(int clienteId, String nombre, String identificacion, String correo, boolean activo)
    {
        this.clienteId = clienteId;
        this.nombre = nombre;
        this.identificacion = identificacion;
        this.correo = correo;
        this.activo = activo;
    }

    // Getters y Setters
    public int getClienteId() { return clienteId; }
    public void setClienteId(int clienteId) { this.clienteId = clienteId; }

    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }

    public String getIdentificacion() { return identificacion; }
    public void setIdentificacion(String identificacion) { this.identificacion = identificacion; }

    public String getCorreo() { return correo; }
    public void setCorreo(String correo) { this.correo = correo; }

    public boolean isActivo() { return activo; }
    public void setActivo(boolean activo) { this.activo = activo; }

    public List<Servicio> getServicios() { return servicios; }
    public void setServicios(List<Servicio> servicios) { this.servicios = servicios; }
}