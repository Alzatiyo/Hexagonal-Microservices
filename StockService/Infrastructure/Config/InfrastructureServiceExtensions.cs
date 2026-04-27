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
    ///     Extension methods for registering Infrastructure and Application services.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        ///     Registers all Infrastructure, Application, Domain services and the database context.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Mappers
            services.AddScoped<IProductMapper, ProductMapper>();

            // Repositories
            services.AddScoped<IProductRepositoryPort, ProductAdapter>();

            // Message Broker audit events (product-events)
            services.AddSingleton<IEventPublisherPort, RabbitMqPublisher>();

            // Message Broker restock requests (stock-restock-requests - SuppliersMS)
            services.AddSingleton<IRestockPublisherPort, RestockRabbitMqPublisher>();

            // Use Cases
            services.AddScoped<IProductUseCasePort, ProductUseCase>();

            // Domain Services
            services.AddScoped<ProductService>();

            return services;
        }
    }
}
