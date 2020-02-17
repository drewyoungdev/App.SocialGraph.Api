using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IRecommendationsRepository
    {
        Task<User[]> GetUserRecommendationsBasedOnFollows(string userId);
        Task<User[]> GetUserRecommendationsBasedOnLikes(string userId);
        Task<UserTweet[]> GetTweetRecommendationsBasedOnFollows(string userId);
        Task<UserTweet[]> GetTweetRecommendationsBasedOnLikes(string userId);
    }
}
