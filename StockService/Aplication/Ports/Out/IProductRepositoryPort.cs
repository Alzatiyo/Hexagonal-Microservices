using Domain.Models;

namespace Aplication.Ports.Out
{
    /// <summary>
    ///     This interface defines the contract for a product repository.
    /// </summary>
    public interface IProductRepositoryPort
    {
        /// <summary>
        ///     Get a product by id from the database, this is method GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product?> GetByIdAsync(Guid id);

        /// <summary>
        ///     Get all products from the database, this is method GET
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        ///     Add a new product to the database, this is method POST
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> SaveAsync(Product product);

        /// <summary>
        ///     Update a product in the database, this is method PUT
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> UpdateAsync(Product product);

        /// <summary>
        ///     Delete a product from the database, this is method DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
