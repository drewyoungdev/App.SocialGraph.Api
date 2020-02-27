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
        Task<long> GetFollowersCount(string userId);
        Task<User[]> GetFollowing(string userId);
        Task<long> GetFollowingCount(string userId);
        Task Follow(string sourceUserId, string destinationUserId);
        Task Unfollow(string sourceId, string destinationId);
    }
}
