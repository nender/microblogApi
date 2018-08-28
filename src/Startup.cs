using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using microblogApi.Models;
using microblogApi.Crypto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace microblogApi
{
    public class Startup
    {
        readonly IConfiguration Configuration;
        public Startup(IConfiguration config) {
            Configuration = config;
        }

        public static void CustomConfigureServices(IServiceCollection services, byte[] tokenKey)
        {
            services.AddDbContext<MicropostContext>(opt => opt.UseSqlite("Data Source=data.db"));

            services.AddScoped<PasswordHasher>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
                    };
                });

            services.AddMvc();
        }

        public void ConfigureServices(IServiceCollection services) {
            var key = Convert.FromBase64String(Configuration["SecretKey"]);
            CustomConfigureServices(services, key);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
