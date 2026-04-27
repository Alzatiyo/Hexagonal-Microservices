using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;

namespace Domain.Services
{
    /// <summary>
    ///     This class represents a service for managing products
    /// </summary>
    public class ProductService
    {
        /// <summary>
        ///     This method evaluates the stock status of a product and updates its status accordingly.
        /// </summary>
        /// <param name="product"></param>
        public void EvaluateStockStatus(Product product)
        {
            product.Status = product.Stock > product.Stockminimum
                ? ProductStatus.Activo
                : ProductStatus.ReabastecimientoPendiente;
        }

        /// <summary>
        ///     This method validates the existence of the product and registers an exist of the product.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        /// <exception cref="DomainException"></exception>
        public Product RegisterExit(int quantity, Product product)
        {
            if (quantity <= 0)
                throw new DomainException("La cantidad debe ser mayor a cero.");

            if (quantity > product.Stock)
                throw new DomainException(
                    $"Stock insuficiente. Stock actual: {product.Stock}, solicitado: {quantity}.");

            return product.RegisterExit(quantity);
        }
    }
}
