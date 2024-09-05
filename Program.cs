using InnoMarkets.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Esta Clase funciona para Interacctuar con la base de datos en la que se almacena los datos de la pagina
builder.Services.AddSingleton(new Contexto(builder.Configuration.GetConnectionString("conexion")));
//En caso que no exista una Autenticacion, el usuario tenga que autenticarse
//Sistema de Logeo Atrevez de Cooki
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option=>
{
    option.LoginPath="/Cuenta/Login";
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
//Ridirige al usuario en caso que no tenga un acceso a un Controlador
app.UseStatusCodePagesWithRedirects("~/Homd/Index");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
