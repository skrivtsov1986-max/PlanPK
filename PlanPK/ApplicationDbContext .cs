using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlanPK
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Issues> Issues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder_)
        {
            base.OnModelCreating(builder_);

            builder_.Entity<Issues>().ToTable("Issues");
        }

    }
}
