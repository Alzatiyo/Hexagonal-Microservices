using Application.Ports.In;
using Application.Ports.Out;
using Application.UseCases;
using Domain.Services;
using Infrastructure.Adapters.Messaging;
using Infrastructure.Adapters.Persistence.Mongo;
using Infrastructure.Mappers;
using Infrastructure.Mappers.Interface;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure.Config
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
            services.AddSingleton<MongoDbContext>();

            // Mappers
            services.AddScoped<IAuditMapper, AuditMapper>();

            // Repositories (outbound adapter)
            services.AddScoped<IAuditRepositoryPort, AuditAdapter>();

            // Use Cases (inbound port)
            services.AddScoped<IAuditUseCasePort, AuditUseCase>();

            // Domain Services
            services.AddScoped<AuditDomainService>();

            // RabbitMQ connection singleton — API v6 síncrona, igual que Stock_Hexagonal
            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",
                    Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = configuration["RabbitMQ:Username"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest"
                };
                return factory.CreateConnection();
            });

            // Consumer (inbound adapter) — BackgroundService
            services.AddHostedService<RabbitMqConsumerAdapter>();

            return services;
        }
    }
}
