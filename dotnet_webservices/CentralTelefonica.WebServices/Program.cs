using CoreWCF;
using CoreWCF.Configuration;
using MongoDB.Driver;
using CentralTelefonica.WebServices.WS_Autenticacion.Contracts;
using CentralTelefonica.WebServices.WS_Autenticacion.Services;
using CentralTelefonica.WebServices.WS_Proveedor.Contracts;
using CentralTelefonica.WebServices.WS_Proveedor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoConnection")));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("central_tg_mongo"));

builder.Services.AddSingleton<AutenticacionService>();
builder.Services.AddSingleton<IAutenticacionService>(sp =>
    sp.GetRequiredService<AutenticacionService>());

builder.Services.AddSingleton<ProveedorService>();
builder.Services.AddSingleton<IProveedorService>(sp =>
    sp.GetRequiredService<ProveedorService>());

builder.Services.AddServiceModelServices();

var app = builder.Build();

app.UseServiceModel(b =>
{
    b.AddService<AutenticacionService>();
    b.AddServiceEndpoint<AutenticacionService, IAutenticacionService>(
        new BasicHttpBinding(),
        "/AutenticacionService");

    b.AddService<ProveedorService>();
    b.AddServiceEndpoint<ProveedorService, IProveedorService>(
        new BasicHttpBinding(),
        "/ProveedorService");
});

app.Run();