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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
