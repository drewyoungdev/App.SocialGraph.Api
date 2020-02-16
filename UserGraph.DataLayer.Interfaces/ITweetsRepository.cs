using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface ITweetsRepository
    {
        Task<Tweet[]> GetAllTweets();
        Task<Tweet> GetTweetById(string tweetId);
        Task<Tweet[]> GetTweetsByUserId(string userId);
        Task Tweet(string userId, Tweet tweet);

        Task<User[]> GetLikes(string tweetId);
        Task<int> GetLikesCount(string tweetId);
        Task Like(string sourceUserId, string destinationTweetId);
        Task Unlike(string sourceUserId, string destinationTweetId);

        // TODO: Get Posts/Tweets Liked By Users You Follow
    }
}
