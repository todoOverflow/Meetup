using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Value>()
                .HasData(
                    new Value { Id = 1, Name = "value 101" },
                    new Value { Id = 2, Name = "value 102" },
                    new Value { Id = 3, Name = "value 103" }
                );

            builder.Entity<UserActivity>()
                .HasKey(ua => new { ua.AppUserId, ua.ActivityId });

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.AppUser)
                .WithMany(appuser => appuser.UserActivities)
                .HasForeignKey(ua => ua.AppUserId);

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.Activity)
                .WithMany(act => act.UserActivities)
                .HasForeignKey(ua => ua.ActivityId);

        }
    }
}
