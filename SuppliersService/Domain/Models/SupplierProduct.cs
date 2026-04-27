namespace Domain.Models
{
    /// <summary>
    /// Define la relación y las condiciones comerciales entre un Proveedor y un Producto del sistema de inventario.
    /// </summary>
    public class SupplierProduct
    {
        public Guid Id { get; set; }
        
        public Guid SupplierId { get; set; }
        
        /// <summary>
        /// ID del producto proveniente de StockPro (Referencia externa para mantener desacoplamiento).
        /// </summary>
        public Guid ExternalProductId { get; set; }
        
        /// <summary>
        /// Nombre del producto desnormalizado para consultas rápidas.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// Precio unitario ofrecido por este proveedor.
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Indica si el proveedor actualmente tiene capacidad para despachar este producto.
        /// </summary>
        public bool IsAvailable { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdateAt { get; set; }
    }
}
