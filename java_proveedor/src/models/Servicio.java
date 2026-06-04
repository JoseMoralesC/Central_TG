package java_proveedor.src.models;

import java.util.Date;

public class Servicio
{
    //Atributos
    private String idServicio; //Consultar ya que aparece como dato generado automáticamente en vez de uno ingresado por el usuario
    private String idCliente; //Consultar ya que no aparece en el diagrama de clases pero es necesario para relacionar el servicio con un cliente específico
    private String telefono; //Consultar ya que no aparece en el diagrama de clases pero es un dato comúnmente asociado a un servicio
    private String tipo_servicio;
    private boolean activo;
    private Date fecha_registro; 

    //Constructor
    public Servicio()
    {
        
    }

    // Getters y Setters
    public String getIdServicio() {
        return idServicio;
    }

    public void setIdServicio(String idServicio) {
        this.idServicio = idServicio;
    }

    public String getIdCliente() {
        return idCliente;
    }

    public void setIdCliente(String idCliente) {
        this.idCliente = idCliente;
    }

    public String getTelefono() {
        return telefono;
    }

    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    public String getTipo_servicio() {
        return tipo_servicio;
    }

    public void setTipo_servicio(String tipo_servicio) {
        this.tipo_servicio = tipo_servicio;
    }

    public boolean isActivo() {
        return activo;
    }

    public void setActivo(boolean activo) {
        this.activo = activo;
    }

    public Date getFecha_registro() {
        return fecha_registro;
    }

    public void setFecha_registro(Date fecha_registro) {
        this.fecha_registro = fecha_registro;
    }
}
