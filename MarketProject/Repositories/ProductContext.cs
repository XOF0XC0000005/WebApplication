using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.Repositories
{
    public class ProductContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        public ProductContext()
        {

        }

        public ProductContext(DbContextOptions<ProductContext> dbc) : base(dbc)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=REVA-L; Database=MarketDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;")
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasKey(x => x.Id).HasName("ProductID");
                entity.HasIndex(x => x.Name).IsUnique();

                entity.Property(e => e.Name)
                .HasColumnName("ProductName")
                .HasMaxLength(255)
                .IsRequired();

                entity.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired();

                entity.Property(e => e.Cost)
                .HasColumnName("Cost")
                .IsRequired();

                entity.HasOne(x => x.ProductGroup)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.Id)
                .HasConstraintName("GroupToProduct");
            });

            modelBuilder.Entity<ProductGroup>(entity =>
            {
                entity.ToTable("ProductGroups");

                entity.HasKey(x => x.Id).HasName("GroupId");
                entity.HasIndex(x => x.Name).IsUnique();

                entity.Property(e => e.Name)
                .HasColumnName("ProductName")
                .HasMaxLength(255);
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storage");

                entity.HasKey(x => x.Id).HasName("StorageId");

                entity.Property(e => e.Name).HasColumnName("StorageName");
                entity.Property(e => e.Count).HasColumnName("ProductCount");

                entity.HasMany(x => x.Products).WithMany(m => m.Storages).UsingEntity(j => j.ToTable("ProductStorage"));
            });
        }
    }
}
