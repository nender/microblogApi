using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using microblogApi.Models;

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize]
    public class MicropostsController : ControllerBase {
        readonly MicropostContext Db;

        public MicropostsController(MicropostContext db) {
            Db = db;
        }

        [HttpGet]
        public IActionResult Index()
            => Ok(Db.Microposts);

        [HttpGet("{id}")]
        public IActionResult Show(long id) {
            var micropost = Db.Microposts.Find(id);
            if (micropost != null)
                return Ok(micropost);
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult Create([FromBody] MicropostRequest request) {
            var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var user = Db.Users.Single(u => u.Email == email);
            Db.Entry(user).Collection(u => u.Microposts).Load();

            var post = new Micropost { Content = request.Content };
            TryValidateModel(post);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            user.Microposts.Add(post);
            Db.SaveChanges();

            return Ok(post);
        }

        [HttpDelete("{id}")]
        public IActionResult Destroy(long id) {
            var post = Db.Microposts.Find(id);
            Db.Microposts.Remove(post);
            Db.SaveChanges();

            return Ok(post);
        }
    }

    public class MicropostRequest {
        public string Content { get; set; }
    }
}