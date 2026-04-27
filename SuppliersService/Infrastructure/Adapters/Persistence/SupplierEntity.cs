using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    /// Entidad de base de datos para la tabla Suppliers.
    /// Utiliza DataAnnotations para mapear directamente con Entity Framework Core.
    /// </summary>
    public class SupplierEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int DeliveryDays { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdateAt { get; set; }

        /// <summary>
        /// Propiedad de navegación para Entity Framework. Representa la relación 1 a N con SupplierProducts.
        /// </summary>
        public ICollection<SupplierProductEntity> Products { get; set; } = new List<SupplierProductEntity>();
    }
}
