using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Repositories;

namespace OpenPOS.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/");
            });

            services.AddDataProtection()
                .PersistKeysToDbContext<PosContext>()
                .SetApplicationName("OpenPOS");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub);
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
            services.AddDbContext<PosContext>(builder =>
            {
                builder.UseNpgsql(Configuration.GetConnectionString("Default"));
            });
            
            services.AddIdentity<PosUser, IdentityRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<PosContext>()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>();
            services.AddAutoMapper((_, automapper) =>
            {
                automapper.EnableNullPropagationForQueryMapping = true;
                automapper.AllowNullCollections = true;
                automapper.AddCollectionMappers();
                automapper.AddProfile<AutoMapperProfile>();
            }, typeof(AutoMapperProfile).Assembly);

            services.AddHttpContextAccessor();

            services.AddScoped<IStoresRepository, StoresRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<IUnitsRepository, UnitsRepository>();
            services.AddScoped<IFirmsRepository, FirmsRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}