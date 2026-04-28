using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra todos los servicios de Infrastructure, Application y Domain
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers y Swagger (los controllers est�n en Infrastructure)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Infrastructure.Adapters.Rest.ProductController).Assembly);

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}



app.UseSwagger();
app.UseSwaggerUI();

// Usar CORS antes de Authorization
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();