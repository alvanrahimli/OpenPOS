using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Identity.Services;

namespace OpenPOS.Identity
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
                options.Conventions.AuthorizeFolder("/Account");
            });
            services.AddControllersWithViews();
            
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

            services.AddScoped<IEmailSender, MockEmailSender>();
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
                endpoints.MapControllers();
            });
        }
    }
}