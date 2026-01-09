using BitcoinClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Post> Posts => Set<Post>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>(b =>
            {
                b.Property(p => p.TextContent).IsRequired();
                b.Property(p => p.ImagePaths).HasColumnType("jsonb");
                b.Property(p => p.Platforms).HasColumnType("jsonb");
                b.HasOne(p => p.AdminUser)
                    .WithMany()
                    .HasForeignKey(p => p.AdminUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
