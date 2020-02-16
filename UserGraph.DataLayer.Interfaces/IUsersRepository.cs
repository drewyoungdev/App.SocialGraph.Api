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

        // TODO: Get Users Followed by Users You Follow (n-steps away where n = 1) // https://neo4j.com/blog/social-networks-in-the-database-using-a-graph-database/
    }
}
