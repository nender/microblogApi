using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace microblogApi.Controllers {

    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize]
    public class MicropostsController : ControllerBase {
    }
}