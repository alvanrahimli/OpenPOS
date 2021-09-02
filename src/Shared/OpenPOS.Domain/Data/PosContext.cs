using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Models;

namespace OpenPOS.Domain.Data
{
    public class PosContext : IdentityDbContext<PosUser>, IDataProtectionKeyContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public PosContext(DbContextOptions<PosContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PosUser>()
                .HasOne(u => u.SelectedStore)
                .WithOne(s => s.SelectorUser) // TODO: Change this for multiple user selectors for next v2
                .HasForeignKey<PosUser>(u => u.SelectedStoreId);

            builder.Entity<Store>()
                .HasOne(s => s.User)
                .WithMany(u => u.Stores)
                .HasForeignKey(store => store.UserId);
            
            // TODO: Enum conversion

            base.OnModelCreating(builder);
        }
    }
}