using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace farm2plate.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<OrderStatusChange> OrderStatusChanges { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SOrder> SOrders { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
