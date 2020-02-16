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

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> Get(string userId)
            => await _usersRepository.GetUserById(userId);

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
            => Created("", await _usersRepository.CreateUser(user));

        [HttpGet("{userId}/following")]
        public async Task<ActionResult<User[]>> GetFollowing(string userId)
            => await _usersRepository.GetFollowing(userId);

        [HttpGet("{userId}/followers")]
        public async Task<ActionResult<User[]>> GetFollowers(string userId)
            => await _usersRepository.GetFollowers(userId);

        [HttpPost("{sourceUserId}/follow/{destinationUserId}")]
        public async Task<IActionResult> Follow(string sourceUserId, string destinationUserId)
        {
            await _usersRepository.Follow(sourceUserId, destinationUserId);

            return Accepted();
        }

        [HttpPost("{sourceUserId}/unfollow/{destinationId}")]
        public async Task<IActionResult> Unfollow(string sourceUserId, string destinationUserId)
        {
            await _usersRepository.Unfollow(sourceUserId, destinationUserId);

            return Accepted();
        }
    }
}
