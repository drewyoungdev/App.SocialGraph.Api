using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    // TODO: Add Follower and Following Count
    // TODO: Rename Followers/Following method?
    public interface IUsersRepository
    {
        Task<User[]> GetAllUsers();
        Task<User> GetUserById(string userId);
        Task<User> CreateUser(User user);

        Task<User[]> GetFollowers(string userId);
        // Task<int> GetFollowersCount(string userId);
        Task<User[]> GetFollowing(string userId);
        // Task<int> GetFollowingCount(string userId);
        Task Follow(string sourceUserId, string destinationUserId);
        Task Unfollow(string sourceId, string destinationId);
    }
}
