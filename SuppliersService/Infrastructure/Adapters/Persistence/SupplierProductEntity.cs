using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    /// Entidad de base de datos para la tabla intermedia SupplierProducts.
    /// </summary>
    public class SupplierProductEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Identificador del producto en el sistema StockPro. No es una Foreign Key real en la BD.
        /// </summary>
        [Required]
        public Guid ExternalProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdateAt { get; set; }

        /// <summary>
        /// Propiedad de navegación hacia el proveedor dueño de este producto.
        /// </summary>
        [ForeignKey(nameof(SupplierId))]
        public SupplierEntity Supplier { get; set; } = null!;
    }
}
