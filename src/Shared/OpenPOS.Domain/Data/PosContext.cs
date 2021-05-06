using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Models;

namespace OpenPOS.Domain.Data
{
    public class PosContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public PosContext(DbContextOptions<PosContext> options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}