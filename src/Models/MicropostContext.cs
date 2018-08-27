using Microsoft.EntityFrameworkCore;

namespace microblogApi.Models {
    public class MicropostContext : DbContext {
        public MicropostContext(DbContextOptions<MicropostContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}