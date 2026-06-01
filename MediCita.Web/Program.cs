using ClinPiura.Web.Data;
using ClinPiura.Web.Servicios.Contrato;
using ClinPiura.Web.Servicios.Implementacion;   // ← Solo este para ChatbotService
using MediCita.Web.Servicios;
using MediCita.Web.Servicios.Contrato;
using MediCita.Web.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. CONFIGURACIÓN DE BASE DE DATOS
// =============================================
builder.Services.AddDbContext<ClinicaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

// =============================================
// 2. REGISTRO DE SERVICIOS
// =============================================
builder.Services.AddControllersWithViews();

// Servicios de negocio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<IAdminEstadisticas, AdminService>();
builder.Services.AddScoped<IReporte, ReporteService>();
builder.Services.AddScoped<IAdminUsuariosService, AdminUsuariosService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<ISeedService, SeedService>();

// === CHATBOT SERVICE ===
builder.Services.AddHttpClient();                    // Necesario para HttpClient
builder.Services.AddScoped<IChatbotService, ChatbotService>();

builder.Services.AddHttpContextAccessor();

// =============================================
// 3. AUTENTICACIÓN Y SESIÓN
// =============================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index";
        options.LogoutPath = "/Acceso/Salir";
        options.AccessDeniedPath = "/Home/Denegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =============================================
// 4. CONSTRUCCIÓN DE LA APP
// =============================================
var app = builder.Build();

// Seeding inicial
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seedService = services.GetRequiredService<ISeedService>();
        await seedService.CrearUsuariosInicialesAsync();   // Mejor usar await
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al ejecutar el seeding de datos.");
    }
}

// =============================================
// 5. MIDDLEWARES
// =============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();