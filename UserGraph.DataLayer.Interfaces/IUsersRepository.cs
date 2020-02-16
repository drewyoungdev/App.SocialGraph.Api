using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IUsersRepository
    {
        Task<User[]> GetAllUsers();
        Task<User> GetUser(string id);
        Task<User[]> GetFollowers(string id);
        Task<User[]> GetFollowing(string id);
        Task Follow(string sourceId, string destinationId);
        Task Unfollow(string sourceId, string destinationId);
    }
}
