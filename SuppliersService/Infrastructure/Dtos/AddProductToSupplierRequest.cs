using System;

namespace Infrastructure.Dtos
{
    /// <summary>
    /// DTO para asociar un producto externo a un proveedor.
    /// </summary>
    public class AddProductToSupplierRequest
    {
        public Guid ExternalProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}
