using Domain.Enums;
using Domain.Models;
using Infrastructure.Adapters.Persistence;
using Infrastructure.Mappers.Interface;

namespace Infrastructure.Mappers
{
    /// <summary>
    /// Implementación del mapeador para transformar datos entre la capa de Persistencia (EF Core) y la capa de Dominio pura.
    /// </summary>
    public class SupplierMapper : ISupplierMapper
    {
        public Supplier ToDomain(SupplierEntity entity)
        {
            return new Domain.Builders.SupplierBuilder()
                .WithId(entity.Id)
                .WithName(entity.Name)
                .WithContactName(entity.ContactName)
                .WithEmail(entity.Email)
                .WithPhone(entity.Phone)
                .WithDeliveryDays(entity.DeliveryDays)
                .WithStatus(entity.Status ? SupplierStatus.Activo : SupplierStatus.Inactivo)
                .WithCreatedAt(entity.CreatedAt)
                .WithUpdateAt(entity.UpdateAt)
                .WithProducts(entity.Products?.Select(ToProductDomain).ToList() ?? new List<SupplierProduct>())
                .Build();
        }

        public SupplierEntity ToEntity(Supplier domain)
        {
            return new SupplierEntityBuilder()
                .WithId(domain.Id)
                .WithName(domain.Name)
                .WithContactName(domain.ContactName)
                .WithEmail(domain.Email)
                .WithPhone(domain.Phone)
                .WithDeliveryDays(domain.DeliveryDays)
                .WithStatus(domain.Status == SupplierStatus.Activo)
                .WithCreatedAt(domain.CreatedAt)
                .WithUpdateAt(domain.UpdateAt)
                .WithProducts(domain.Products?.Select(ToProductEntity).ToList() ?? new List<SupplierProductEntity>())
                .Build();
        }

        public SupplierProduct ToProductDomain(SupplierProductEntity entity)
        {
            return new Domain.Builders.SupplierProductBuilder()
                .WithId(entity.Id)
                .WithSupplierId(entity.SupplierId)
                .WithExternalProductId(entity.ExternalProductId)
                .WithProductName(entity.ProductName)
                .WithUnitPrice(entity.UnitPrice)
                .WithIsAvailable(entity.IsAvailable)
                .WithCreatedAt(entity.CreatedAt)
                .WithUpdateAt(entity.UpdateAt)
                .Build();
        }

        public SupplierProductEntity ToProductEntity(SupplierProduct domain)
        {
            return new SupplierProductEntity
            {
                Id = domain.Id,
                SupplierId = domain.SupplierId,
                ExternalProductId = domain.ExternalProductId,
                ProductName = domain.ProductName,
                UnitPrice = domain.UnitPrice,
                IsAvailable = domain.IsAvailable,
                CreatedAt = domain.CreatedAt,
                UpdateAt = domain.UpdateAt
            };
        }
    }
}
