using Domain.Builders;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Adapters.Persistence;
using Infrastructure.Mappers.Interface;

namespace Infrastructure.Mappers
{
    /// <summary>
    ///     This class is responsible for mapping between the product domain model and the product entity model.
    /// </summary>
    public class ProductMapper : IProductMapper
    {
        /// <summary>
        ///     This method maps a product domain model to a product entity model.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public ProductEntity ToEntity(Product domain) =>
            new ProductEntityBuilder()
                .WithId(domain.Id)
                .WithName(domain.Name)
                .WithDescripcion(domain.Descripcion)
                .WithStock(domain.Stock)
                .WithStockminimum(domain.Stockminimum)
                .WithPrice(domain.Price)
                .WithStatus(domain.Status == ProductStatus.Activo)
                .WithCreatedAt(domain.CreatedAt)
                .WithUpdateAt(domain.UpdateAt)
                .Build();

        /// <summary>
        ///     This method maps a product entity model to a product domain model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Product ToDomain(ProductEntity entity) =>
            new ProductBuilder()
                .WithId(entity.Id)
                .WithName(entity.Name)
                .WithDescripcion(entity.Descripcion)
                .WithStock(entity.Stock)
                .WithStockminimum(entity.Stockminimum)
                .WithPrice(entity.Price)
                .WithStatus(entity.Status
                    ? ProductStatus.Activo
                    : ProductStatus.ReabastecimientoPendiente)
                .WithCreatedAt(entity.CreatedAt)
                .WithUpdateAt(entity.UpdateAt)
                .Build();
    }
}
