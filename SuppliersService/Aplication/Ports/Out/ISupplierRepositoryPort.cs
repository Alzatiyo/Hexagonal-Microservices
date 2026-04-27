using Domain.Models;

namespace Aplication.Ports.Out
{
    /// <summary>
    /// Contrato de repositorio para la entidad Supplier.
    /// </summary>
    public interface ISupplierRepositoryPort
    {
        /// <summary>
        /// Obtiene un proveedor por su identificador único.
        /// </summary>
        Task<Supplier?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene todos los proveedores registrados.
        /// </summary>
        Task<IEnumerable<Supplier>> GetAllAsync();

        /// <summary>
        /// Obtiene los proveedores asociados a un producto externo específico.
        /// </summary>
        Task<IEnumerable<Supplier>> GetAllByProductAsync(Guid externalProductId);

        /// <summary>
        /// Guarda un nuevo proveedor en la base de datos.
        /// </summary>
        Task<Supplier?> SaveAsync(Supplier supplier);

        /// <summary>
        /// Actualiza la información de un proveedor existente.
        /// </summary>
        Task<Supplier?> UpdateAsync(Supplier supplier);

        /// <summary>
        /// Elimina un proveedor.
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Guarda la relación entre un proveedor y un producto externo.
        /// </summary>
        Task<SupplierProduct?> SaveProductAsync(SupplierProduct supplierProduct);
    }
}
