using Aplication.Ports.In;
using Aplication.Ports.Out;
using Aplication.UseCases;
using Domain.Services;
using Infrastructure.Adapters.Messaging;
using Infrastructure.Adapters.Persistence;
using Infrastructure.Mappers;
using Infrastructure.Mappers.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config
{
    /// <summary>
    ///     Métodos de extensión para inyectar todas las dependencias del microservicio en el contenedor de IoC.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            // Mappers
            services.AddScoped<ISupplierMapper, SupplierMapper>();

            // Repositories
            services.AddScoped<ISupplierRepositoryPort, SupplierAdapter>();

            // Use Cases
            services.AddScoped<ISupplierUseCasePort, SupplierUseCase>();

            // Domain Services
            services.AddScoped<SupplierService>();

            // RabbitMQ Consumer — escucha "stock-restock-requests" de Stock_Hexagonal
            services.AddHostedService<RestockRequestConsumer>();

            return services;
        }
    }
}