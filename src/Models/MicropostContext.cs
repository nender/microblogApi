using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace microblogApi.Models {
    public class MicropostContext : IdentityDbContext<User, Role, long> {
        public MicropostContext(DbContextOptions<MicropostContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}