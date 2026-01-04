using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.DAL
{
    public class GCADbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleLineItem> SaleLineItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseLineItem> PurchaseLineItems { get; set; }

        public GCADbContext(DbContextOptions<GCADbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if needed, e.g. composite keys or ON DELETE logic
            
            // Example: Cascade delete behavior
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.LineItems)
                .WithOne(li => li.Sale)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.LineItems)
                .WithOne(li => li.Purchase)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
