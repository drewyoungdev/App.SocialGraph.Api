using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api.Controllers
{
    [Route("api/[controller]")]
    public class TweetsController : ControllerBase
    {
        private ITweetsRepository _tweetsRepository;

        public TweetsController(ITweetsRepository tweetsRepository)
        {
            _tweetsRepository = tweetsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Tweet[]>> Get()
            => await _tweetsRepository.GetAllTweets();

        [HttpGet("{tweetId}")]
        public async Task<ActionResult<Tweet>> GetByTweetId(string tweetId)
            => await _tweetsRepository.GetTweetById(tweetId);

        [HttpGet("{tweetId}/likes/count")]
        public async Task<ActionResult<int>> GetLikesCount(string tweetId)
            => await _tweetsRepository.GetLikesCount(tweetId);

        [HttpGet("{tweetId}/likes")]
        public async Task<ActionResult<User[]>> GetLikes(string tweetId)
            => await _tweetsRepository.GetLikes(tweetId);

        [HttpGet("/api/users/{userId}/tweets")]
        public async Task<ActionResult<Tweet[]>> GetByUserId(string userId)
            => await _tweetsRepository.GetTweetsByUserId(userId);

        [HttpPost("/api/users/{userId}/tweet")]
        public async Task<IActionResult> Tweet(string userId, [FromBody] Tweet tweet)
        {
            await _tweetsRepository.Tweet(userId, tweet);

            return Accepted();
        }

        [HttpPost("/api/users/{sourceUserId}/like/{destinationTweetId}")]
        public async Task<IActionResult> Like(string sourceUserId, string destinationTweetId)
        {
            await _tweetsRepository.Like(sourceUserId, destinationTweetId);

            return Accepted();
        }

        [HttpPost("/api/users/{sourceUserId}/unlike/{destinationTweetId}")]
        public async Task<IActionResult> Unlike(string sourceUserId, string destinationTweetId)
        {
            await _tweetsRepository.Unlike(sourceUserId, destinationTweetId);

            return Accepted();
        }
    }
}
