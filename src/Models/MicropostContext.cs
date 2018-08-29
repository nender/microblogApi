using Microsoft.EntityFrameworkCore;

namespace microblogApi.Models {
    public class MicropostContext : DbContext {
        public MicropostContext(DbContextOptions<MicropostContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                        .HasIndex(p => p.Email)
                        .IsUnique();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Micropost> Microposts { get; set; }
    }
}