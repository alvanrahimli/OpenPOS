using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Repositories;

namespace OpenPOS.Sale
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizePage("/sale");
            });
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            
            services.AddDataProtection()
                .PersistKeysToDbContext<PosContext>()
                .SetApplicationName("OpenPOS");
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".o.p";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Domain = Configuration["Cookie:Domain"]; 
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(31);
            });
            services.AddAntiforgery(opts => {
                opts.Cookie.SameSite = SameSiteMode.Lax;
            });
            
            services.AddAutoMapper((_, automapper) =>
            {
                automapper.EnableNullPropagationForQueryMapping = true;
                automapper.AllowNullCollections = true;
                automapper.AddCollectionMappers();
                automapper.AddProfile<AutoMapperProfile>();
            }, typeof(AutoMapperProfile).Assembly);

            services.AddDbContextFactory<PosContext>(builder =>
            {
                builder.UseNpgsql(Configuration.GetConnectionString("Default"));
            });
            
            // services.AddScoped(p => p.GetRequiredService<IDbContextFactory<PosContext>>()
            //     .CreateDbContext());
            services.AddDbContext<PosContext>(builder =>
            {
                builder.UseSqlite(Configuration.GetConnectionString("Default"));
            });

            services.AddIdentity<PosUser, IdentityRole>()
                .AddEntityFrameworkStores<PosContext>()
                .AddDefaultTokenProviders();

            services.AddDistributedMemoryCache();

            services.AddScoped<ITransactionsRepository, TransactionsRepository>();
            services.AddScoped<IStoresRepository, StoresRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<IUnitsRepository, UnitsRepository>();
            services.AddScoped<IFirmsRepository, FirmsRepository>();
            services.AddScoped<IClientsRepository, ClientsRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
