using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace microblogApi.Models {
    public class MicropostContext : DbContext {
        public MicropostContext(DbContextOptions<MicropostContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                        .HasIndex(p => p.Email)
                        .IsUnique();
        }

        public override int SaveChanges()
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            var now = DateTime.UtcNow;

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as ITimestamps;

                if (entry.State == EntityState.Added)
                    entity.CreatedAt = now;

                entity.UpdatedAt = now;
            }

            return base.SaveChanges();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Micropost> Microposts { get; set; }
    }
}