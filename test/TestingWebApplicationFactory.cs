using microblogApi.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace microblogApi.Test {
    public class TestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
            where TStartup : class
    {
        public TestingWebApplicationFactory() { }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                const string testdb = "test.db";
                if (File.Exists(testdb))
                    File.Delete(testdb);

                services.AddDbContext<MicropostContext>(opt => opt.UseSqlite($"Data Source={testdb}"));

                services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<MicropostContext>()
                    .AddDefaultTokenProviders();

                services.AddMvc();

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MicropostContext>();

                    // Ensure the database is created.
                    var result = db.Database.EnsureCreated();
                }
            });
        }
    }
}