using microblogApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using microblogApi.Crypto;
using Microsoft.AspNetCore.Authorization;

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase {
        readonly MicropostContext Db;
        readonly IConfiguration Configuration;
        readonly PasswordHasher PasswordHasher;
        public UsersController(MicropostContext context, IConfiguration config, PasswordHasher pwHash) {
            Db = context;
            Configuration = config;
            PasswordHasher = pwHash;
        }

        [HttpGet]
        public IActionResult GetUserIndex() {
            return Ok(Db.Users.Select(x => x.ToViewModel()));
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(long id) {
            var user = Db.Users.Find(id);
            if (user != null)
                return Ok(user.ToViewModel());
            else
                return NotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create([FromBody]CreateUserRequest person) {
            var user = new User
            {
                UserName = person.username,
                Email = person.email,
                PasswordHash = PasswordHasher.HashPassword(person.password)
            };

            if (TryValidateModel(user)) {
                Db.Users.Add(user);
                Db.SaveChanges();
                return Ok(user.ToViewModel());
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Update(long id, [FromBody]UpdateUserRequest postData) {
            var user = Db.Users.Find(id);
            bool authorized = User.HasClaim(c => c.Type == "email" && c.Value == user.Email);
            if (!authorized)
                return Unauthorized();

            user.UserName = postData.username ?? user.UserName;
            user.Email = postData.email ?? user.Email;
            user.PasswordHash = PasswordHasher.HashPassword(postData.password) ?? user.PasswordHash;

            TryValidateModel(user);
            if (ModelState.IsValid) {
                Db.SaveChanges();
                return Ok(user.ToViewModel());
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("/api/authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody]AuthenticationRequest auth) {
            var user = Db.Users.Where(x => x.Email == auth.email).FirstOrDefault();
            var authOk = PasswordHasher.CheckPasword(user?.PasswordHash, auth.password);
            if (!authOk)
                return BadRequest("Could not verify password");

            var claims = new[] {
                new Claim("email", auth.email)
            };

            var rawKey = Convert.FromBase64String(Configuration["SecretKey"]);
            var key = new SymmetricSecurityKey(rawKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpDelete("{id}")]
        public IActionResult Destroy(long id) {
            var user = Db.Users.Find(id);
            bool authorized = User.HasClaim(c => c.Type == "email" && c.Value == user.Email);
            if (!authorized)
                return Unauthorized();
            
            Db.Users.Remove(user);
            Db.SaveChanges();

            return Ok(user.ToViewModel());
        }
    }

    public static class UserExtensions {
        public static UserViewModel ToViewModel(this User usr)
            => new UserViewModel(usr);
    }

    public class UserViewModel {
        readonly User _user;
        public UserViewModel(User user) {
            _user = user;
        }

        public string username => _user.UserName;
        public string email => _user.Email;
        public long id => _user.Id;
    }

    public class AuthenticationRequest {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }

    public class UpdateUserRequest
    {
        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }

    public class CreateUserRequest
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}