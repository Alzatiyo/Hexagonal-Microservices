using Domain.Models;
using Infrastructure.Adapters.Persistence;

namespace Infrastructure.Mappers.Interface
{
    /// <summary>
    ///     This interface defines the contract for a product mapper, this is used to map between the domain model and the entity model.
    /// </summary>
    public interface IProductMapper
    {
        /// <summary>
        ///     This method maps a product domain model to a product entity model.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        ProductEntity ToEntity(Product domain);

        /// <summary>
        ///     This method maps a product entity model to a product domain model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Product ToDomain(ProductEntity entity);
    }
}
