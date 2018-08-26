using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using microblogApi.Models;
using microblogApi.Test.Data;

namespace microblogApi.Test {
    public class MicroblogWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public MicroblogWebApplicationFactory() { }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                const string testdb = "test.db";
                if (File.Exists(testdb))
                    File.Delete(testdb);

                services.AddDbContext<MicropostContext>(opt => opt.UseSqlite($"Data Source={testdb}"));

                Startup.ConfigureServices(services);

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MicropostContext>();
                    var userMan = scopedServices.GetRequiredService<UserManager<User>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                    foreach (var user in UserFixtures.Users) {
                        var result = await userMan.CreateAsync(user, "Fo0b@r");
                        if (!result.Succeeded)
                            throw new Exception("Couldn't create seed user");
                    }
                }
            });
        }
    }
}