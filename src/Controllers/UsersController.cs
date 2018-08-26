using microblogApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        readonly MicropostContext Db;
        readonly MicroblogUserManager UserManager;
        public UsersController(MicropostContext context, MicroblogUserManager userManager) {
            Db = context;
            UserManager = userManager;
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
        public async Task<IActionResult> Create([FromBody]UserRequest person) {
            var user = new User { UserName = person.username, Email = person.email };

            if (!TryValidateModel(user))
                return BadRequest(ModelState);

            var result = await UserManager.CreateAsync(user, person.password);
            if (result.Succeeded)
                return Created("", user.ToViewModel());
            else
                return BadRequest(result.Errors);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody]UserRequest postData) {
            var user = Db.Users.Find(id);
            if (user == null)
                return NotFound();

            user.UserName = postData.username ?? user.UserName;
            user.Email = postData.email ?? user.Email;
            TryValidateModel(user);

            IdentityResult pwChangeResult = null;
            if (postData.password != null) {
                pwChangeResult = await UserManager.ChangePasswordAsync(user, postData.password);
            }

            bool pwChangeError = pwChangeResult?.Errors.Any() ?? false;
            if (ModelState.IsValid && !pwChangeError) {
                Db.SaveChanges();
                return Ok();
            } else {
                return BadRequest(ModelState);
            }
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
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserRequest
    {
        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }
}