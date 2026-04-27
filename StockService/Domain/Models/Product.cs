using Domain.Builders;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models
{
    /// <summary>
    ///     This class represents a product in the system.
    /// </summary>
    public class Product
    {
        /// <summary>
        ///     The unique identifier for the product.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The name of the product, this is a required.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        ///     The description of the product, this is a required.
        /// </summary>
        public required string Descripcion { get; set; }

        /// <summary>
        ///     The stock of the product, this is quantity of the product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        ///     The stock minimum of the product, this is the minimum quantity of the product that should be in stock.
        /// </summary>
        public int Stockminimum { get; set; }

        /// <summary>
        ///     The price of the product, this is the cost of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     The status of the product, indicates if the product is active or not.
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        ///     The product creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     The product update date.
        /// </summary>
        public DateTime UpdateAt { get; set; }

        /// <summary>
        ///    This method registers an exit of the product, this is when a product is sold or removed from stock.
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="DomainException"></exception>
        public Product RegisterExit(int quantity)
        {
            return new ProductBuilder()
                .WithId(this.Id)
                .WithName(this.Name)
                .WithDescripcion(this.Descripcion)
                .WithStock(this.Stock - quantity)
                .WithStockminimum(this.Stockminimum)
                .WithPrice(this.Price)
                .WithStatus((this.Stock - quantity) <= this.Stockminimum
                    ? ProductStatus.ReabastecimientoPendiente
                    : ProductStatus.Activo)
                .WithCreatedAt(this.CreatedAt)
                .WithUpdateAt(this.UpdateAt)
                .Build();
        }
    }
}
