using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UserGraph.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}
