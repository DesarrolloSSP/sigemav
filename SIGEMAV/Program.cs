using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SIGEMAV.Models.Data;
using SIGEMAV.Services.Implementations.Financieros;
using SIGEMAV.Services.Implementations.Maquinaria;
using SIGEMAV.Services.Implementations.SIA;
using SIGEMAV.Services.Interfaces.Financieros;
using SIGEMAV.Services.Interfaces.Maquinaria;
using SIGEMAV.Services.Interfaces.SIA;
using static SIGEMAV.Services.Interfaces.Maquinaria.IMaquinariaService;
using System.IO;



// Configurar licencia
QuestPDF.Settings.License = LicenseType.Community;


var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
#region Base de Datos



// Registrar la fuente Gotham desde la carpeta wwwroot/Fonts en tu proyecto
FontManager.RegisterFont(new MemoryStream(File.ReadAllBytes(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Fonts", "Gotham-Book.ttf"))));
FontManager.RegisterFont(new MemoryStream(File.ReadAllBytes(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Fonts", "Gotham-Bold.ttf"))));
FontManager.RegisterFont(new MemoryStream(File.ReadAllBytes(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Fonts", "Gotham-Ultra.otf"))));
FontManager.RegisterFont(new MemoryStream(File.ReadAllBytes(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Fonts", "Gotham-UltraItalic.ttf"))));




builder.Services.AddDbContext<BdSigeMavContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("conexion")));

#endregion

#region Configuración de carga de archivos

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 15 * 1024 * 1024;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 15 * 1024 * 1024;
});

#endregion

#region Servicios

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IMaquinariaService, MaquinariaService>();
builder.Services.AddScoped<MaquinariaService>();
builder.Services.AddScoped<IDispService, DispService>();
builder.Services.AddScoped<IObjetoGastoCatalogoService, ObjetoGastoCatalogoService>();
builder.Services.AddScoped<IClaveAdministrativaService, ClaveAdministrativaService>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IProyectoAreaService, ProyectoAreaService>();
builder.Services.AddScoped<IOdpService, OdpService>();
builder.Services.AddScoped<IDspService, DspService>();
builder.Services.AddScoped<IOrdenDePago, OrdenDePagoService>();
builder.Services.AddHttpClient<ISiaService, SiaService>();


#endregion

#region MVC

builder.Services.AddControllersWithViews();

#endregion

#region Autenticación

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Account/Login";

        // Página personalizada 403
        options.AccessDeniedPath = "/Error/403";

        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

#endregion

var app = builder.Build();

#region Middleware

if (!app.Environment.IsDevelopment())
{
    // Página personalizada 500
    app.UseExceptionHandler("/Error/500");

    app.UseHsts();
}

// Manejo automático de 404, 403, etc.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

#endregion

#region Rutas

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();