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

        public DbSet<ImportedMemberProfile> ImportedMemberProfiles => Set<ImportedMemberProfile>();

        public DbSet<Post> Posts => Set<Post>();

        public DbSet<PostPublishResult> PostPublishResults => Set<PostPublishResult>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>(b =>
            {
                b.Property(p => p.TextContent).IsRequired();
                if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
                {
                    b.Property(p => p.ImagePaths).HasColumnType("jsonb");
                    b.Property(p => p.Platforms).HasColumnType("jsonb");
                }
                b.HasOne(p => p.AdminUser)
                    .WithMany()
                    .HasForeignKey(p => p.AdminUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PostPublishResult>(b =>
            {
                b.Property(p => p.Platform).IsRequired();
                b.HasOne(p => p.Post)
                    .WithMany()
                    .HasForeignKey(p => p.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(p => new { p.PostId, p.Platform });
            });

            builder.Entity<ImportedMemberProfile>(b =>
            {
                b.Property(p => p.FullName).IsRequired();
                b.Property(p => p.DiscordNickname).IsRequired();
                b.Property(p => p.Position).IsRequired();
                b.Property(p => p.TotalContributionsRaw).IsRequired();
                b.Property(p => p.VolunteerInterests).IsRequired();
                b.Property(p => p.StreetAddress).IsRequired();
                b.Property(p => p.City).IsRequired();
                b.Property(p => p.Region).IsRequired();
                b.Property(p => p.PostalCode).IsRequired();
                b.Property(p => p.SecondaryEmail).IsRequired();
                b.Property(p => p.Notes).IsRequired();
                b.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(p => p.UserId).IsUnique();
            });
        }
    }
}
