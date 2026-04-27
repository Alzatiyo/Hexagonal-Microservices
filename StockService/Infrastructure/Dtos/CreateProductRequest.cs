namespace Infrastructure.Dtos
{
    /// <summary>
    ///     This class represents the object trnasfer for creating a product.
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        ///     The name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The description of the product.
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        ///     The stock of the product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        ///     The stock minimum of the product.
        /// </summary>
        public int Stockminimum { get; set; }

        /// <summary>
        ///     The price of the product.
        /// </summary>
        public decimal Price { get; set; }
    }
}