using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(builder.Configuration.GetConnectionString("MongoConnection")));

        builder.Services.AddSingleton<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("central_tg_mongo"));

        var app = builder.Build();

        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
        app.MapPost("/autenticacion/login", (AutenticacionRequest request) =>
            Results.Ok(new { exito = true, mensaje = "Servicio listo", request }));
        app.MapPost("/proveedor/facturacion", (FacturacionRequest request) =>
            Results.Ok(new { exito = true, mensaje = "Servicio listo", request }));

        app.Run();
    }
}

public class AutenticacionRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public int Tipo { get; set; }
}

public class FacturacionRequest
{
    public string Identificacion { get; set; } = string.Empty;
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
}
