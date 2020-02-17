using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private IRecommendationsRepository _recommendationsRepository;

        public RecommendationsController(IRecommendationsRepository recommendationsRepository)
        {
            _recommendationsRepository = recommendationsRepository;
        }

        [HttpGet("users/basedOnFollows")]
        public async Task<ActionResult<User[]>> GetUserRecommendationsBasedOnFollows(string userId)
            => await _recommendationsRepository.GetUserRecommendationsBasedOnFollows(userId);

        [HttpGet("users/basedOnLikes")]
        public async Task<ActionResult<User[]>> GetUserRecommendationsBasedOnLikes(string userId)
            => await _recommendationsRepository.GetUserRecommendationsBasedOnLikes(userId);

        [HttpGet("tweets/basedOnFollows")]
        public async Task<ActionResult<Tweet[]>> GetTweetRecommendationsBasedOnFollows(string userId)
            => await _recommendationsRepository.GetTweetRecommendationsBasedOnFollows(userId);

        [HttpGet("tweets/basedOnLikes")]
        public async Task<ActionResult<Tweet[]>> GetTweetRecommendationsBasedOnLikes(string userId)
            => await _recommendationsRepository.GetTweetRecommendationsBasedOnLikes(userId);
    }
}
