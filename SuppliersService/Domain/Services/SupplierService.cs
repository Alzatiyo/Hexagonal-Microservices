using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;

namespace Domain.Services
{
    /// <summary>
    /// Servicio de Dominio que encapsula las reglas de negocio complejas sobre proveedores.
    /// </summary>
    public class SupplierService
    {
        /// <summary>
        /// Valida las condiciones iniciales de un proveedor antes de crearlo o actualizarlo.
        /// </summary>
        /// <param name="supplier">Entidad proveedor a evaluar.</param>
        /// <exception cref="DomainException">Se lanza si el proveedor no cumple con los tiempos de entrega mínimos.</exception>
        public void EvaluateSupplierStatus(Supplier supplier)
        {
            if (supplier.DeliveryDays <= 0)
                throw new DomainException("Los días de entrega deben ser mayores a cero.");

            supplier.Status = SupplierStatus.Activo;
        }

        /// <summary>
        /// Filtra y ordena los proveedores disponibles para un producto específico, priorizando la rapidez de entrega.
        /// </summary>
        /// <param name="suppliers">Lista de todos los proveedores que tienen el producto.</param>
        /// <param name="externalProductId">ID del producto que se necesita reabastecer.</param>
        /// <returns>Una lista estructurada de proveedores viables, donde el primero (más rápido) es marcado como recomendado.</returns>
        public IEnumerable<(Supplier Supplier, SupplierProduct SupplierProduct, bool IsRecommended)> GetAvailableForProduct(
            IEnumerable<Supplier> suppliers,
            Guid externalProductId)
        {
            var available = suppliers
                .Where(s => s.Status == SupplierStatus.Activo)
                .Select(s => new
                {
                    Supplier = s,
                    SupplierProduct = s.Products.FirstOrDefault(p =>
                        p.ExternalProductId == externalProductId && p.IsAvailable)
                })
                .Where(x => x.SupplierProduct is not null)
                .OrderBy(x => x.Supplier.DeliveryDays) // Regla de negocio central: Prioridad al más rápido
                .ToList();

            if (!available.Any())
                return Enumerable.Empty<(Supplier, SupplierProduct, bool)>();

            return available.Select((x, index) => (
                x.Supplier,
                x.SupplierProduct!,
                IsRecommended: index == 0 // El primer elemento de la lista ordenada se marca como recomendado
            ));
        }
    }
}
