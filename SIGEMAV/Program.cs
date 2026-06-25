using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Services.Implementations.Financieros;
using SIGEMAV.Services.Implementations.Maquinaria;
using SIGEMAV.Services.Implementations.SIA;
using SIGEMAV.Services.Interfaces.Financieros;
using SIGEMAV.Services.Interfaces.Maquinaria;
using SIGEMAV.Services.Interfaces.SIA;
using System.Reflection.Emit;

var builder = WebApplication.CreateBuilder(args);







//////SERVICIOS
builder.Services.AddDbContext<BdSigeMavContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("conexion")));

builder.Services.AddScoped<IMaquinariaService, MaquinariaService>();


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 15 * 1024 * 1024; // 15 MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 15 * 1024 * 1024;
});



builder.Services.AddScoped<MaquinariaService>();
////SERVICIOS



builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<BdSigeMavContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

builder.Services.AddScoped<IDispService, DispService>();

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddScoped<IObjetoGastoCatalogoService, ObjetoGastoCatalogoService>();
builder.Services.AddScoped<IClaveAdministrativaService, ClaveAdministrativaService>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IProyectoAreaService, ProyectoAreaService>();
builder.Services.AddScoped<IOdpService, OdpService>();
builder.Services.AddScoped<IDspService, DspService>();
builder.Services.AddHttpClient<ISiaService, SiaService>();
builder.Services.AddScoped<IOrdenDePago, OrdenDePagoService>();


builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath =
            "/Account/Account/Login";

        options.AccessDeniedPath =
            "/Account/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromDays(30);

        options.SlidingExpiration = true;
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

