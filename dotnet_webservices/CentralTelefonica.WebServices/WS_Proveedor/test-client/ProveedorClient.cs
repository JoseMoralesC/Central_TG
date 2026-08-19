using System.ServiceModel;

public class ProveedorClient
{
    public static void Main()
    {
        var endpointAddress = new EndpointAddress("http://localhost:8080/ProveedorService");
        var binding = new BasicHttpBinding();
        var factory = new ChannelFactory<IProveedorService>(binding, endpointAddress);
        var client = factory.CreateChannel();

        var response = client.ObtenerFacturaPostpago("123456789", DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"));
        Console.WriteLine($"Exito: {response.Exito}");
        Console.WriteLine($"Mensaje: {response.Mensaje}");
        Console.WriteLine($"Total: {response.TotalFacturar}");
    }
}
