using Aplication.Ports.In;
using Aplication.Ports.Out;
using Domain.Models;
using Domain.Services;

namespace Aplication.UseCases
{
    /// <summary>
    /// Implementación del caso de uso principal para la gestión de Proveedores.
    /// Orquesta las interacciones entre los puertos de entrada/salida y las reglas de dominio.
    /// </summary>
    public class SupplierUseCase : ISupplierUseCasePort
    {
        private readonly ISupplierRepositoryPort _repository;
        private readonly SupplierService _supplierService;

        public SupplierUseCase(
            ISupplierRepositoryPort repository,
            SupplierService supplierService)
        {
            _repository = repository;
            _supplierService = supplierService;
        }

        /// <summary>
        /// Crea un nuevo proveedor y le asigna su estado inicial evaluando las reglas de negocio.
        /// </summary>
        public async Task<Supplier?> CreateAsync(Supplier supplier)
        {
            supplier.Id = Guid.NewGuid();
            supplier.CreatedAt = DateTime.UtcNow;
            supplier.UpdateAt = DateTime.UtcNow;

            // La regla de dominio valida que el proveedor sea viable antes de guardarlo
            _supplierService.EvaluateSupplierStatus(supplier);

            return await _repository.SaveAsync(supplier);
        }

        /// <summary>
        /// Obtiene un proveedor por su identificador.
        /// </summary>
        public async Task<Supplier?> GetByIdAsync(Guid id) =>
            await _repository.GetByIdAsync(id);

        /// <summary>
        /// Obtiene todos los proveedores almacenados.
        /// </summary>
        public async Task<IEnumerable<Supplier>> GetAllAsync() =>
            await _repository.GetAllAsync();

        /// <summary>
        /// Actualiza la información de un proveedor existente.
        /// </summary>
        public async Task<Supplier?> UpdateAsync(Guid id, Supplier supplier)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
                return null;

            supplier.Id = id;
            supplier.UpdateAt = DateTime.UtcNow;

            _supplierService.EvaluateSupplierStatus(supplier);

            return await _repository.UpdateAsync(supplier);
        }

        /// <summary>
        /// Elimina un proveedor.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id) =>
            await _repository.DeleteAsync(id);

        /// <summary>
        /// Vincula un producto externo a un proveedor existente.
        /// </summary>
        public async Task<Supplier?> AddProductAsync(Guid supplierId, SupplierProduct supplierProduct)
        {
            var supplier = await _repository.GetByIdAsync(supplierId);
            if (supplier is null)
                return null;

            supplierProduct.Id = Guid.NewGuid();
            supplierProduct.SupplierId = supplierId;
            supplierProduct.CreatedAt = DateTime.UtcNow;
            supplierProduct.UpdateAt = DateTime.UtcNow;

            await _repository.SaveProductAsync(supplierProduct);

            return await _repository.GetByIdAsync(supplierId);
        }

        /// <summary>
        /// Encuentra y clasifica los proveedores disponibles para surtir un producto en estado de reabastecimiento.
        /// </summary>
        public async Task<IEnumerable<object>> GetAvailableByProductAsync(Guid externalProductId)
        {
            var suppliers = await _repository.GetAllByProductAsync(externalProductId);
            var recommendations = _supplierService.GetAvailableForProduct(suppliers, externalProductId);

            return recommendations.Select(r => new
            {
                SupplierId = r.Supplier.Id,
                Name = r.Supplier.Name,
                ContactName = r.Supplier.ContactName,
                Email = r.Supplier.Email,
                Phone = r.Supplier.Phone,
                DeliveryDays = r.Supplier.DeliveryDays,
                UnitPrice = r.SupplierProduct.UnitPrice,
                IsRecommended = r.IsRecommended
            });
        }
    }
}
