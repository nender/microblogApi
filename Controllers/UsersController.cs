using microblogApi.Model;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace microblogApi.Controllers {
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase{
        MicropostContext Db;
        public UsersController(MicropostContext context)
        {
            Db = context;
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
        public IActionResult Create([FromBody] User user) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (Db.Users.Any(u => u.Email == user.Email))
                return BadRequest("Email must be unique");
            
            Db.Users.Add(user);
            Db.SaveChanges();

            return Created("", user);
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
    }
}