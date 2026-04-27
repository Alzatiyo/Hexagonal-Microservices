using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Ports.Out;
using Domain.Models;
using Infrastructure.Config;
using Infrastructure.Mappers.Interface;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Infrastructure.Adapters.Persistence
{
    /// <summary>
    ///     This class represents the product adapter.
    /// </summary>
    public class ProductAdapter : IProductRepositoryPort
    {
        private readonly AppDbContext _context;
        private readonly IProductMapper _mapper;

        public ProductAdapter(AppDbContext context, IProductMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///     This method gets a product by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity is null)
                return null;
            return _mapper.ToDomain(entity);
        }

        /// <summary>
        ///     This method gets all products from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var entities = await _context.Products.ToListAsync();
            return entities.Select(_mapper.ToDomain);
        }

        /// <summary>
        ///     This method adds a new product to the database.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task<Product?> SaveAsync(Product domain)
        {
            var entity = _mapper.ToEntity(domain);
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.ToDomain(entity);
        }

        /// <summary>
        ///     This method updates a product in the database.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task<Product?> UpdateAsync(Product domain)
        {
            var entity = await _context.Products.FindAsync(domain.Id);
            if (entity is null) 
                return null;

            entity.Name = domain.Name;
            entity.Descripcion = domain.Descripcion;
            entity.Stock = domain.Stock;
            entity.Stockminimum = domain.Stockminimum;
            entity.Price = domain.Price;
            entity.Status = domain.Status == ProductStatus.Activo;
            entity.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.ToDomain(entity);
        }

        /// <summary>
        ///     This method deletes a product from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity is null) 
                return false;

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
