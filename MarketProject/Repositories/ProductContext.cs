using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.Repositories
{
    public class ProductContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        private string _connectionString;
        public ProductContext()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public ProductContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ProductContext(DbContextOptions<ProductContext> dbc) : base(dbc)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasKey(x => x.Id).HasName("product_pkey");
                //entity.HasIndex(x => x.Name).IsUnique();

                entity.Property(e => e.Name)
                .HasColumnName("product_name")
                .HasMaxLength(255)
                .IsRequired();

                entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(255)
                .IsRequired();

                entity.Property(e => e.Cost)
                .HasColumnName("cost")
                .IsRequired();

                entity.HasOne(x => x.ProductGroup)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.Id)
                .HasConstraintName("group_to_product");
            });

            modelBuilder.Entity<ProductGroup>(entity =>
            {
                entity.ToTable("product_groups");

                entity.HasKey(x => x.Id).HasName("group_pkey");
                entity.HasIndex(x => x.Name).IsUnique();

                entity.Property(e => e.Name)
                .HasColumnName("product_name")
                .HasMaxLength(255);
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.ToTable("Storage");

                entity.HasKey(x => x.Id).HasName("storage_pkey");

                entity.Property(e => e.Name).HasColumnName("storage_name");
                entity.Property(e => e.Count).HasColumnName("product_count");

                entity.HasMany(x => x.Products).WithMany(m => m.Storages).UsingEntity(j => j.ToTable("product_storage"));
            });
        }
    }
}
