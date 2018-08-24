using Microsoft.EntityFrameworkCore;

namespace microblogApi.Model {
    public class MicropostContext : DbContext {
        public MicropostContext(DbContextOptions<MicropostContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                .HasIndex(o => o.Email)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }
    }
}