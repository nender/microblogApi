using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using microblogApi.Models;
using microblogApi.Test.Data;
using microblogApi.Crypto;
using System.Security.Cryptography;

namespace microblogApi.Test {
    public class MicroblogWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public MicroblogWebApplicationFactory() { }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var hasher = new PasswordHasher();

            builder.ConfigureServices(services => {
                const string testdb = "test.db";
                services.AddDbContext<MicropostContext>(opt => opt.UseSqlite($"Data Source={testdb}"));

                using (var scope = services.BuildServiceProvider().CreateScope()) {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MicropostContext>();

                    if (File.Exists(testdb))
                        File.Delete(testdb);
                    db.Database.EnsureCreated();
                    foreach (var user in UserFixtures.Users) {
                        user.PasswordHash = hasher.HashPassword("Fo0b@r");
                        db.Users.Add(user);
                    }
                    db.SaveChanges();
                }
            });
        }
    }
}