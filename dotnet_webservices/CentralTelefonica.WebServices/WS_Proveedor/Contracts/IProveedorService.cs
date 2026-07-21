using System.Runtime.Serialization;
using CentralTelefonica.WebServices.WS_Proveedor.Models;

[ServiceContract]
public interface IProveedorService
{
    [OperationContract]
    FacturacionResponse ObtenerFacturaPostpago(string identificacion, DateTime inicio, DateTime fin);
}

[ServiceContract]
public interface IAutenticacionService
{
    [OperationContract]
    AutenticacionResponse AutenticarUsuario(AutenticacionRequest request);
}

[DataContract]
public class AutenticacionRequest
{
    [DataMember]
    public string Usuario { get; set; } = "";

    [DataMember]
    public string Contrasena { get; set; } = "";

    [DataMember]
    public int Tipo { get; set; }
}

[DataContract]
public class AutenticacionResponse
{
    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = "";

    [DataMember]
    public string Token { get; set; } = "";
}

public class AutenticacionService : IAutenticacionService
{
    public AutenticacionResponse AutenticarUsuario(AutenticacionRequest request)
    {
        var payload = new
        {
            usuario = request.Usuario,
            contrasena = request.Contrasena,
            tipo = request.Tipo
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload);

        //Llamada aqui al servicio de Bases de Datos
        //luego la respuesta se transforma de nuevo a SOAP

        return new AutenticacionResponse
        {
            Exito = true,
            Mensaje = "Autenticación exitosa",
            Token = "abc123"
        };
    }
}
