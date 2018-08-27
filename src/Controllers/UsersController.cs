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

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        readonly MicropostContext Db;
        readonly IConfiguration Configuration;
        public UsersController(MicropostContext context, IConfiguration config) {
            Db = context;
            Configuration = config;
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
        public async Task<IActionResult> Create([FromBody]CreateUserRequest person) {
            var user = new User { UserName = person.username, Email = person.email };

            if (!TryValidateModel(user))
                return BadRequest(ModelState);

            throw new NotImplementedException();
            //var result = await UserManager.CreateAsync(user, person.password);
            //if (result.Succeeded)
            //    return Created("", user.ToViewModel());
            //else
            //    return BadRequest(result.Errors);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody]UpdateUserRequest postData) {
            var user = Db.Users.Find(id);
            if (user == null)
                return NotFound();

            user.UserName = postData.username ?? user.UserName;
            user.Email = postData.email ?? user.Email;
            TryValidateModel(user);

            throw new NotImplementedException();
            //IdentityResult pwChangeResult = null;
            //if (postData.password != null) {
            //    pwChangeResult = await UserManager.ChangePasswordAsync(user, postData.password);
            //}

            //bool pwChangeError = pwChangeResult?.Errors.Any() ?? false;
            //if (ModelState.IsValid && !pwChangeError) {
            //    Db.SaveChanges();
            //    return Ok();
            //} else {
            //    return BadRequest(ModelState);
            //}
        }

        [HttpPost("/api/authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticationRequest auth) {
            throw new NotImplementedException();
            //var user = UserManager.FindByEmailAsync(auth.email).Result;
            //if (user == null)
            //    return NotFound("Could not find user with that email");

            //var authOk = UserManager.CheckPasswordAsync(user, auth.password).Result;
            //if (!authOk)
            //    return BadRequest("Could not verify password");

            //var claims = new[] {
            //    new Claim(ClaimTypes.Email, auth.email)
            //};

            //var rawKey = Convert.FromBase64String(Configuration["SecretKey"]);
            //var key = new SymmetricSecurityKey(rawKey);
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    claims: claims,
            //    expires: DateTime.Now.AddYears(1),
            //    signingCredentials: creds
            //);

            //return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpDelete("{id}")]
        public IActionResult Destroy(long id) {
            var user = Db.Users.Find(id);
            if (user == null)
                return BadRequest("No such user");
            
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