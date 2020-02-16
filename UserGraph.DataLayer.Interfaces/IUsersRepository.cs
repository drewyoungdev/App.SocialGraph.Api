using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IUsersRepository
    {
        Task<User[]> GetAllUsers();
        Task<User> GetUser(string id);
        Task<User> CreateUser(User user);
        Task<User[]> GetFollowers(string id);
        Task<User[]> GetFollowing(string id);
        Task Follow(string sourceId, string destinationId);
        Task Unfollow(string sourceId, string destinationId);
        // TODO: Create Posts/Tweets
        // TODO: Like Posts/Tweets
        // TODO: Get Posts/Tweets Liked By Users You Follow
        // TODO: Get Users Followed by Users You Follow (n-steps away where n = 1) // https://neo4j.com/blog/social-networks-in-the-database-using-a-graph-database/
        // TODO: Init method
    }
}
