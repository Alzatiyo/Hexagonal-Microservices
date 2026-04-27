using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    ///     This class represents the product table in the database.
    /// </summary>
    public class ProductEntity
    {
        /// <summary>
        ///     The unique identifier for the product, primary key in the database.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     The name of the product.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The description of the product.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        ///     The stock quantity of the product.
        /// </summary>
        [Column(TypeName = "int")]
        public int Stock { get; set; }

        /// <summary>
        ///     The minimum stock threshold of the product.
        /// </summary>
        [Column(TypeName = "int")]
        public int Stockminimum { get; set; }

        /// <summary>
        ///     The price of the product.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        ///     The status of the product, stored as bit in the database.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        ///     The product creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     The product last update date.
        /// </summary>
        public DateTime UpdateAt { get; set; }
    }
}
