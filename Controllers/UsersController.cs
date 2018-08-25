using microblogApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace microblogApi.Controllers {
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase{
        readonly MicropostContext Db;
        readonly UserManager<User> UserManager;
        public UsersController(MicropostContext context, UserManager<User> userManager)
        {
            Db = context;
            UserManager = userManager;
        }

        [HttpGet]
        public IActionResult GetUserIndex() {
            return Ok(Db.Users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(long id) {
            var user = Db.Users.Find(id);
            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult Create([FromBody]PersonResponse person) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User { UserName = person.username, Email = person.email };
            var result = UserManager.CreateAsync(user, person.password).Result;
            if (result.Succeeded) {
                return Created("", user);
            } else {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Destroy(long id) {
            var user = Db.Users.Find(id);
            if (user == null)
                return BadRequest("No such user");
            
            Db.Users.Remove(user);
            Db.SaveChanges();

            return Ok(user);
        }

        public class PersonResponse
        {
            [Required]
            public string username {get; set;}
            [Required]
            public string email {get; set;}
            [Required]
            public string password {get;set;}
        }
    }
}