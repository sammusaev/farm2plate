using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace farm2plate
{
    public class Startup
    {
        private async Task SetupRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleTitles = { "Admin", "Vendor", "Customer" };
            IdentityResult roleResult;

            // Checking if roles exist
            foreach (var roleTitle in roleTitles) {
                var roleExists = await RoleManager.RoleExistsAsync(roleTitle);
                // Create them if they don't 
                if (!roleExists) {
                    Console.WriteLine("Adding role: ", roleTitle);
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleTitle));
                }
            }

            // Setting up a default super admin
            // This requires that xyz in Configuration["AppSettings:xyz"] 
            // be present in appsettings.json

            // Check if superadmin exists
            var _superAdmin = await UserManager.FindByEmailAsync(Configuration["AppSettings:AdminEmail"]);
            
            if (_superAdmin == null) {
                // Create a superadmin if it doesn't already exist
                var SuperAdminApplicationUser = new ApplicationUser {
                    UserName = Configuration["AppSettings:AdminEmail"],
                    Email = Configuration["AppSettings:AdminEmail"],
                    FirstName = Configuration["AppSettings:AdminFirstName"],
                    LastName = Configuration["AppSettings:AdminLastName"],
                };

                string SuperAdminPassword = Configuration["AppSettings:AdminPassword"];

                var createSuperAdmin = await UserManager.CreateAsync(SuperAdminApplicationUser, SuperAdminPassword);
                if (createSuperAdmin.Succeeded) {
                    await UserManager.AddToRoleAsync(SuperAdminApplicationUser, "Admin");
                    Console.WriteLine("SuperAdmin has been created");
                } 
                else {
                    Console.WriteLine("Failed to create admin");
                }
            } 
            else
            {
                Console.WriteLine("Admin already exists");
            }

        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:ApplicationDbContextConnection"]));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IServiceProvider serviceProvider,
            ApplicationDbContext context) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseXRay("farm2plate");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            SetupRoles(serviceProvider).Wait();
        }
    }
}
