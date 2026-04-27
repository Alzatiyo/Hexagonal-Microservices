using Domain.Enums;

namespace Domain.Models
{
    /// <summary>
    /// Representa a un Proveedor en el dominio del negocio.
    /// </summary>
    public class Supplier
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string ContactName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// Tiempo promedio en días que tarda el proveedor en entregar un pedido.
        /// Se utiliza para calcular qué proveedor es más óptimo.
        /// </summary>
        public int DeliveryDays { get; set; }
        
        public SupplierStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdateAt { get; set; }
        
        /// <summary>
        /// Lista de productos que este proveedor puede suministrar.
        /// </summary>
        public ICollection<SupplierProduct> Products { get; set; } = new List<SupplierProduct>();

        /// <summary>
        /// Cambia el estado del proveedor a inactivo mediante la reconstrucción de la entidad.
        /// </summary>
        /// <returns>Una nueva instancia de Supplier con el estado actualizado.</returns>
        public Supplier Deactivate()
        {
            return new Builders.SupplierBuilder()
                .WithId(this.Id)
                .WithName(this.Name)
                .WithContactName(this.ContactName)
                .WithEmail(this.Email)
                .WithPhone(this.Phone)
                .WithDeliveryDays(this.DeliveryDays)
                .WithStatus(SupplierStatus.Inactivo)
                .WithCreatedAt(this.CreatedAt)
                .WithUpdateAt(DateTime.UtcNow)
                .WithProducts(this.Products)
                .Build();
        }
    }
}
