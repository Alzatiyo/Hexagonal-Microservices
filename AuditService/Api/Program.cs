using Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);

// Registra todos los servicios de Infrastructure, Application y Domain
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers y Swagger (los controllers est�n en Infrastructure)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Infrastructure.Adapters.Rest.AuditController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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

app.UseAuthorization();
app.MapControllers();
app.Run();