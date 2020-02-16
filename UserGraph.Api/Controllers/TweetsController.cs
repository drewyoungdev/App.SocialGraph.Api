using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api.Controllers
{
    [Route("api/[controller]")]
    public class TweetsController : ControllerBase
    {
        private IUsersRepository _usersRepository;

        public TweetsController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Tweet[]>> Get()
            => await _usersRepository.GetAllTweets();

        [HttpGet("{tweetId}")]
        public async Task<ActionResult<Tweet>> GetByTweetId(string tweetId)
            => await _usersRepository.GetTweetById(tweetId);

        [HttpGet("{tweetId}/likes/count")]
        public async Task<ActionResult<int>> GetLikesCount(string tweetId)
            => await _usersRepository.GetLikesCount(tweetId);

        [HttpGet("{tweetId}/likes")]
        public async Task<ActionResult<User[]>> GetLikes(string tweetId)
            => await _usersRepository.GetLikes(tweetId);

        [HttpGet("/api/users/{userId}/tweets")]
        public async Task<ActionResult<Tweet[]>> GetByUserId(string userId)
            => await _usersRepository.GetTweetsByUserId(userId);

        [HttpPost("/api/users/{userId}/tweet")]
        public async Task<IActionResult> Tweet(string userId, [FromBody] Tweet tweet)
        {
            await _usersRepository.Tweet(userId, tweet);

            return Accepted();
        }

        [HttpPost("/api/users/{sourceUserId}/like/{destinationTweetId}")]
        public async Task<IActionResult> Like(string sourceUserId, string destinationTweetId)
        {
            await _usersRepository.Like(sourceUserId, destinationTweetId);

            return Accepted();
        }

        [HttpPost("/api/users/{sourceUserId}/unlike/{destinationTweetId}")]
        public async Task<IActionResult> Unlike(string sourceUserId, string destinationTweetId)
        {
            await _usersRepository.Unlike(sourceUserId, destinationTweetId);

            return Accepted();
        }
    }
}
