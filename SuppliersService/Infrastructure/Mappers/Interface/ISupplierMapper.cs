using Domain.Models;
using Infrastructure.Adapters.Persistence;

namespace Infrastructure.Mappers.Interface
{
    /// <summary>
    /// Contrato para el mapeo entre las entidades de base de datos y los modelos de dominio.
    /// </summary>
    public interface ISupplierMapper
    {
        Supplier ToDomain(SupplierEntity entity);
        SupplierEntity ToEntity(Supplier domain);
        SupplierProduct ToProductDomain(SupplierProductEntity entity);
        SupplierProductEntity ToProductEntity(SupplierProduct domain);
    }
}
