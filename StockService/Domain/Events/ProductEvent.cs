using Domain.Models;

namespace Domain.Events
{
    /// <summary>
    ///     This class represents a product event, it contains all the information about the product and the action that was performed on it.
    /// </summary>
    public class ProductEvent
    {
        /// <summary>
        ///     The unique identifier for the event.
        /// </summary>
        public Guid EventId { get; private set; } = Guid.NewGuid();

        /// <summary>
        ///    The type of action performed on the product.
        /// </summary>
        public string Action { get; private set; } = string.Empty;

        /// <summary>
        ///     The unique identifier for the product.
        /// </summary>
        public Guid ProductId { get; private set; }

        /// <summary>
        ///     The name of the product, this is a required.
        /// </summary>
        public string ProductName { get; private set; } = string.Empty;

        /// <summary>
        ///     The description of the product, this is a required.
        /// </summary>
        public string ProductDescription { get; private set; } = string.Empty;

        /// <summary>
        ///     The stock of the product, this is quantity of the product.
        /// </summary>
        public int Stock { get; private set; }

        /// <summary>
        ///     The price of the product, this is the cost of the product.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        ///    The status of the product.
        /// </summary>
        public string Status { get; private set; } = string.Empty;

        /// <summary>
        ///    The date and time when the event occurred,.
        /// </summary>
        public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        ///    This method creates a new product event based on the action performed and the product information.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public static ProductEvent Create(ProductAction action, Product product)
        {
            return new ProductEvent
            {
                Action = action.ToString(),
                ProductId = product.Id,
                ProductName = product.Name,
                ProductDescription = product.Descripcion,
                Stock = product.Stock,
                Price = product.Price,
                Status = product.Status.ToString(),
                OccurredAt = DateTime.UtcNow
            };
        }
    }
}