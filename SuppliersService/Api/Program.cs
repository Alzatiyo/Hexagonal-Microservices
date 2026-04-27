using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuracion del servidor web Kestrel para aceptar peticiones externas al contenedor Docker
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5002); // Puerto 5002 para SuppliersService
});

// Configuracion de Controladores y Serializacion JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Inyeccion de dependencias centralizada desde la capa de Infraestructura
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Aplicacion automatica de migraciones de base de datos al iniciar la API
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrio un error al migrar la base de datos durante el arranque.");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Usar CORS antes de Authorization
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();
