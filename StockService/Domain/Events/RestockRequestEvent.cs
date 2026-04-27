namespace Domain.Events
{
    /// <summary>
    ///     Evento publicado en RabbitMQ cuando el stock de un producto cae
    ///     por debajo del mínimo. SuppliersMS lo consume para buscar el
    ///     proveedor más adecuado para el reabastecimiento.
    /// </summary>
    public class RestockRequestEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int CurrentStock { get; init; }
        public int StockMinimum { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

        public static RestockRequestEvent Create(Guid productId, string productName, int currentStock, int stockMinimum)
            => new()
            {
                ProductId = productId,
                ProductName = productName,
                CurrentStock = currentStock,
                StockMinimum = stockMinimum
            };
    }
}