using System.ServiceModel;
using System.ServiceModel.Description;

public class AutenticacionClient
{
    public static void Main()
    {
        var endpointAddress = new EndpointAddress("http://localhost:8080/AutenticacionService");
        var binding = new BasicHttpBinding();
        var factory = new ChannelFactory<IAutenticacionService>(binding, endpointAddress);
        var client = factory.CreateChannel();

        var response = client.AutenticarUsuario("admin", "Password1!", 1);
        Console.WriteLine($"Exito: {response.Exito} - {response.Mensaje}");
    }
}
