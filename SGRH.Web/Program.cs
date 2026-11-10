using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SGRH.BL;
using SGRH.Web.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Controladores y vistas. Todos los formularios POST validan el token antifalsificación.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

// Opciones de negocio, servicios (BL) y repositorios (DAL)
builder.Services.AddApplication(builder.Configuration);

// Servicios propios de la capa de presentación
builder.Services.AddScoped<ICatalogoService, CatalogoService>();

var app = builder.Build();

// Si se configuró SQLite (PC sin SQL Server), se crea la base de datos con datos de ejemplo.
await DatosDemo.InicializarAsync(app.Services, builder.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Fechas, números y moneda con el formato de República Dominicana.
var cultura = new CultureInfo("es-DO");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = [cultura],
    SupportedUICultures = [cultura]
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
