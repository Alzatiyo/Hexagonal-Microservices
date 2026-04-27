using Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);

// Registra todos los servicios de Infrastructure, Application y Domain
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers y Swagger (los controllers están en Infrastructure)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Infrastructure.Adapters.Rest.AuditController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();