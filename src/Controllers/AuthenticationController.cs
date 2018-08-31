using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using microblogApi.Crypto;
using microblogApi.Models;

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        readonly MicropostContext Db;
        readonly IConfiguration Configuration;
        readonly PasswordHasher PasswordHasher;

        public AuthenticationController(MicropostContext db, IConfiguration config, PasswordHasher hasher) {
            Db = db;
            Configuration = config;
            PasswordHasher = hasher;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticationRequest auth) {
            var user = Db.Users.Where(x => x.Email == auth.email).FirstOrDefault();
            var authOk = PasswordHasher.CheckPasword(user?.PasswordHash, auth.password);
            if (!authOk)
                return BadRequest("Could not verify password");

            var claims = new[] {
                new Claim(ClaimTypes.Email, auth.email)
            };

            var rawKey = Convert.FromBase64String(Configuration["SecretKey"]);
            var key = new SymmetricSecurityKey(rawKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenStr);
        }
    }
}