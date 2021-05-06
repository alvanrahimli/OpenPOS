using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OpenPOS.Domain.Data
{
    public class PosContextFactory : IDesignTimeDbContextFactory<PosContext>
    {
        public PosContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DB_ENV") == "Production" ? "Production" : "Development";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                // .AddEnvironmentVariables()
                .Build();
            var connectionString = configuration.GetConnectionString("Default");
            
            var dbContextBuilder = new DbContextOptionsBuilder<PosContext>();
            dbContextBuilder.UseNpgsql(connectionString);

            return new PosContext(dbContextBuilder.Options);
        }
    }
}