package java_proveedor.src.models;

public class Cliente
{
    private String idCliente; //Consultar ya que aparece como dato generado automáticamente en vez de uno ingresado por el usuario
    private String nombre;
    private String email;
    //private String telefono; //Consultar ya que no aparece en el diagrama de clases pero es un dato comúnmente asociado a un cliente
    private boolean activo;

    public Cliente(String idCliente, String nombre, String email, boolean activo)
    {
        this.idCliente = idCliente;
        this.nombre = nombre;
        this.email = email;
        this.activo = activo;
    }

    // Getters y Setters
    public String getNombre()
    {
        return nombre;
    }

    public void setNombre(String nombre)
    {
        this.nombre = nombre;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email)
    {
        this.email = email;
    }

    public boolean getActivo()
    {
        return activo;
    }

    public void setActivo(boolean activo)
    {
        this.activo = activo;
    }
}
