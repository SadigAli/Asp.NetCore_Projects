using Leka.DAL;
using Leka.Models;
using Leka.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options=>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            }).AddIdentity<AppUser,IdentityRole>(opt=> 
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequiredUniqueChars = 0;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

            services.AddHttpContextAccessor();

            services.AddScoped<LayoutService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("areas","{area:exists}/{controller=dashboard}/{action=Index}/{id?}");
            });
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute("Default", "{Controller=home}/{Action=index}/{Id?}");
                endpoints.MapControllerRoute("Default1", "about-us", defaults: new { Controller="Home", Action="About" });
                endpoints.MapControllerRoute("Default2", "contact-us", defaults: new { Controller = "Home", Action = "Contact" });
                endpoints.MapControllerRoute("Default3", "login", defaults: new { Controller = "Account", Action = "login" });
                endpoints.MapControllerRoute("Default4", "register", defaults: new { Controller = "Account", Action = "register" });
                endpoints.MapControllerRoute("Default5", "logout", defaults: new { Controller = "Account", Action = "logout" });
                endpoints.MapControllerRoute("Default6", "profile", defaults: new { Controller = "Account", Action = "resetpassword" });
            });
        }
    }
}
