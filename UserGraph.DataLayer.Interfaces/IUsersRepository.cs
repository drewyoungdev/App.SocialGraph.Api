using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IUsersRepository
    {
        Task<User[]> GetAllUsers();
        Task<User> GetUserById(string userId);
        Task<User> CreateUser(User user);

        Task<User[]> GetFollowers(string userId);
        Task<User[]> GetFollowing(string userId);
        Task Follow(string sourceUserId, string destinationUserId);
        Task Unfollow(string sourceId, string destinationId);

        Task<Tweet[]> GetAllTweets();
        Task<Tweet> GetTweetById(string tweetId);
        Task<Tweet[]> GetTweetsByUserId(string userId);
        Task Tweet(string userId, Tweet tweet);

        Task<User[]> GetLikes(string tweetId);
        Task<int> GetLikesCount(string tweetId);
        Task Like(string sourceUserId, string destinationTweetId);
        Task Unlike(string sourceUserId, string destinationTweetId);

        // TODO: Get Posts/Tweets Liked By Users You Follow
        // TODO: Get Users Followed by Users You Follow (n-steps away where n = 1) // https://neo4j.com/blog/social-networks-in-the-database-using-a-graph-database/
        // TODO: Init method
    }
}
