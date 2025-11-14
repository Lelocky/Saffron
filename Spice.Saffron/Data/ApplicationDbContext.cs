using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Spice.Saffron.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add index on DateOfBirth for better query performance
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.DateOfBirth)
                .HasDatabaseName("IX_AspNetUsers_DateOfBirth");
        }
    }
}