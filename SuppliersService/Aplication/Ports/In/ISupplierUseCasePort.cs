using Domain.Models;

namespace Aplication.Ports.In
{
    /// <summary>
    /// Puerto de entrada para el caso de uso de Proveedores.
    /// </summary>
    public interface ISupplierUseCasePort
    {
        Task<Supplier?> CreateAsync(Supplier supplier);

        Task<Supplier?> GetByIdAsync(Guid id);

        Task<IEnumerable<Supplier>> GetAllAsync();

        Task<Supplier?> UpdateAsync(Guid id, Supplier supplier);

        Task<bool> DeleteAsync(Guid id);

        Task<Supplier?> AddProductAsync(Guid supplierId, SupplierProduct supplierProduct);

        /// <summary>
        /// Devuelve los proveedores disponibles y ordenados para reabastecer un producto externo.
        /// </summary>
        Task<IEnumerable<object>> GetAvailableByProductAsync(Guid externalProductId);
    }
}
