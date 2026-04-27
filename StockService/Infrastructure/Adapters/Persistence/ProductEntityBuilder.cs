namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    ///     Class to build a product entity.
    /// </summary>
    public class ProductEntityBuilder
    {
        private Guid _id;
        private string _name = string.Empty;
        private string _descripcion = string.Empty;
        private int _stock;
        private int _stockminimum;
        private decimal _price;
        private bool _status;
        private DateTime _createdAt;
        private DateTime _updateAt;

        /// <summary>
        ///     Save the unique identifier of the product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        ///     Save the name of the product.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        ///     Save the description of the product.
        /// </summary>
        /// <param name="descripcion"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        /// <summary>
        ///    Save the stock of the product.
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithStock(int stock)
        {
            _stock = stock;
            return this;
        }

        /// <summary>
        ///     Save the minium stock of the product.
        /// </summary>
        /// <param name="stockminimum"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithStockminimum(int stockminimum)
        {
            _stockminimum = stockminimum;
            return this;
        }

        /// <summary>
        ///     Save the price of the product.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        /// <summary>
        ///     Save the status of the product.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithStatus(bool status)
        {
            _status = status;
            return this;
        }

        /// <summary>
        ///     Save the creation date of the product.
        /// </summary>
        /// <param name="createdAt"></param>
        /// <returns></returns>
        public ProductEntityBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        /// <summary>
        ///     Save the update date of the product.
        /// </summary>
        /// <param name="updateAt">The <see cref="DateTime"/> value representing the update timestamp to assign.</param>
        /// <returns>The current <see cref="ProductEntityBuilder"/> instance, allowing for method chaining.</returns>
        public ProductEntityBuilder WithUpdateAt(DateTime updateAt)
        {
            _updateAt = updateAt;
            return this;
        }

        /// <summary>
        ///     Creates a new instance of the product entity.
        /// </summary>
        /// <returns></returns>
        public ProductEntity Build() => new ProductEntity
        {
            Id = _id,
            Name = _name,
            Descripcion = _descripcion,
            Stock = _stock,
            Stockminimum = _stockminimum,
            Price = _price,
            Status = _status,
            CreatedAt = _createdAt,
            UpdateAt = _updateAt
        };
    }
}
