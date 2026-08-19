using PortalCliente.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<IProveedorClienteService, ProveedorClienteSoapClient>();
builder.Services.AddSingleton<IProveedorPortalService, ProveedorPortalSoapClient>();
builder.Services.AddSingleton<IProveedor2Service, Proveedor2SoapClient>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IAutenticacionPortalService, AutenticacionPortalSoapClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
