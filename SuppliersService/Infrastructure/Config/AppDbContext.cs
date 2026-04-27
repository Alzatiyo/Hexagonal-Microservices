using Microsoft.EntityFrameworkCore;
using Infrastructure.Adapters.Persistence;

namespace Infrastructure.Config
{
    /// <summary>
    /// Contexto principal de Entity Framework Core para la base de datos de Proveedores.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SupplierEntity> Suppliers => Set<SupplierEntity>();
        public DbSet<SupplierProductEntity> SupplierProducts => Set<SupplierProductEntity>();

        /// <summary>
        /// Configuración mediante Fluent API para garantizar la integridad referencial y las restricciones.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupplierEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.ContactName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Phone).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Status).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired();

                entity.HasMany(s => s.Products)
                      .WithOne(p => p.Supplier)
                      .HasForeignKey(p => p.SupplierId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Suppliers");
            });

            modelBuilder.Entity<SupplierProductEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ExternalProductId).IsRequired();
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
                
                entity.ToTable("SupplierProducts");
            });
        }
    }
}
