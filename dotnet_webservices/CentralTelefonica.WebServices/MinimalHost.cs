using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CentralTelefonica.WebServices.WS_Autenticacion.Contracts;
using CentralTelefonica.WebServices.WS_Autenticacion.Services;
using CentralTelefonica.WebServices.WS_Proveedor.Contracts;
using CentralTelefonica.WebServices.WS_Proveedor.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoConnection")));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("central_tg_mongo"));

builder.Services.AddSingleton<AutenticacionService>();
builder.Services.AddSingleton<IAutenticacionService>(sp => sp.GetRequiredService<AutenticacionService>());

builder.Services.AddSingleton<ProveedorService>();
builder.Services.AddSingleton<IProveedorService>(sp => sp.GetRequiredService<ProveedorService>());

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost("/autenticacion/login", (AutenticacionRequest request, IAutenticacionService servicio) =>
    Results.Ok(servicio.AutenticarUsuario(request.Usuario, request.Contrasena, request.Tipo)));
app.MapPost("/proveedor/facturacion", (FacturacionRequest request, IProveedorService servicio) =>
    Results.Ok(servicio.ObtenerFacturaPostpago(request.Identificacion, request.Inicio, request.Fin)));

app.Run();

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
