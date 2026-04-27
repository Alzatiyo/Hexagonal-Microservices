using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplication.Ports.Out;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Config;
using Infrastructure.Mappers.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    /// Adaptador de repositorio concreto utilizando Entity Framework Core.
    /// Implementa el puerto de salida definido en la capa de Aplicación.
    /// </summary>
    public class SupplierAdapter : ISupplierRepositoryPort
    {
        private readonly AppDbContext _context;
        private readonly ISupplierMapper _mapper;

        public SupplierAdapter(AppDbContext context, ISupplierMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (entity is null)
                return null;
                
            return _mapper.ToDomain(entity);
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var entities = await _context.Suppliers
                .Include(s => s.Products)
                .ToListAsync();
                
            return entities.Select(_mapper.ToDomain);
        }

        public async Task<IEnumerable<Supplier>> GetAllByProductAsync(Guid externalProductId)
        {
            var entities = await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.Products.Any(p => p.ExternalProductId == externalProductId))
                .ToListAsync();
                
            return entities.Select(_mapper.ToDomain);
        }

        public async Task<Supplier?> SaveAsync(Supplier domain)
        {
            var entity = _mapper.ToEntity(domain);
            await _context.Suppliers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.ToDomain(entity);
        }

        public async Task<Supplier?> UpdateAsync(Supplier domain)
        {
            var entity = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == domain.Id);
                
            if (entity is null) 
                return null;

            entity.Name = domain.Name;
            entity.ContactName = domain.ContactName;
            entity.Email = domain.Email;
            entity.Phone = domain.Phone;
            entity.DeliveryDays = domain.DeliveryDays;
            entity.Status = domain.Status == SupplierStatus.Activo;
            entity.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.ToDomain(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Suppliers.FindAsync(id);
            if (entity is null) 
                return false;

            _context.Suppliers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SupplierProduct?> SaveProductAsync(SupplierProduct supplierProduct)
        {
            var entity = _mapper.ToProductEntity(supplierProduct);
            await _context.SupplierProducts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.ToProductDomain(entity);
        }
    }
}
