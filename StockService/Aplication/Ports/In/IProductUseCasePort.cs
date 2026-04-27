using Domain.Models;

namespace Aplication.Ports.In
{
    /// <summary>
    ///     This interface defines the contract, this is the product use case.
    /// </summary>
    public interface IProductUseCasePort
    {
        /// <summary>
        ///     This method creates a new product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> CreateAsync(Product product);

        /// <summary>
        ///     This method gets a product by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product?> GetByIdAsync(Guid id);

        /// <summary>
        ///     This method gets all products.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        ///     This method updates a product by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> UpdateAsync(Guid id, Product product);

        /// <summary>
        ///     This method deletes a product by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        ///     This method registers an exit of a product, this is used to decrease the stock of a product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<Product?> RegisterExitAsync(Guid id, int quantity);
    }
}
