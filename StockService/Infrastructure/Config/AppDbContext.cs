using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Adapters.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Config
{
    /// <summary>
    ///     This class represents the application's database context.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        ///    This constructor initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        ///     Represents the Products table in the database.
        /// </summary>
        public DbSet<ProductEntity> Products => Set<ProductEntity>();

        /// <summary>
        ///     This method is used to configure the model and the database schema.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Status).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired();

                entity.ToTable("Products");
            });
        }
    }
}
