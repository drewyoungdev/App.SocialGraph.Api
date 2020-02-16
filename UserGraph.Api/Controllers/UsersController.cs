using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<ActionResult<User[]>> Get()
            => await _usersRepository.GetAllUsers();

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(string id)
            => await _usersRepository.GetUser(id);

        [HttpGet("{id}/following")]
        public async Task<ActionResult<User[]>> GetFollowing(string id)
            => await _usersRepository.GetFollowing(id);

        [HttpGet("{id}/followers")]
        public async Task<ActionResult<User[]>> GetFollowers(string id)
            => await _usersRepository.GetFollowers(id);

        [HttpPost("{sourceId}/follow/{destinationId}")]
        public async Task<IActionResult> Follow(string sourceId, string destinationId)
        {
            await _usersRepository.Follow(sourceId, destinationId);

            return Accepted();
        }

        [HttpPost("{sourceId}/unfollow/{destinationId}")]
        public async Task<IActionResult> Unfollow(string sourceId, string destinationId)
        {
            await _usersRepository.Unfollow(sourceId, destinationId);

            return Accepted();
        }
    }
}
