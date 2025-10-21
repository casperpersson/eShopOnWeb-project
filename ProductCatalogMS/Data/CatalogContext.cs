using ProductCatalogMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogMS.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure CatalogItem relationships
            builder.Entity<CatalogItem>()
                .HasOne(c => c.CatalogBrand)
                .WithMany()
                .HasForeignKey(c => c.CatalogBrandId);

            builder.Entity<CatalogItem>()
                .HasOne(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(c => c.CatalogTypeId);

            // Configure decimal precision
            builder.Entity<CatalogItem>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
