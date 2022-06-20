using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DataAcesss.Data
{
    public partial class PUSGS_ProjectContext : IdentityDbContext<AppUser>
    {
        public PUSGS_ProjectContext()
        {
        }

        public PUSGS_ProjectContext(DbContextOptions<PUSGS_ProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<SignUpRequest> SignUpRequests { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProductOrder>().HasKey(pi => new { pi.ProductId, pi.OrderId });
        }
    }
}
