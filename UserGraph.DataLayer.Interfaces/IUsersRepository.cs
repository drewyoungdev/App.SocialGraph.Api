using System.Threading.Tasks;
using UserGraph.Models;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IUsersRepository
    {
        Task<User[]> GetAllUsers();
        Task<User> GetUser(string id);
        Task AddFollowsEdge(User source, User destination);
        Task<string[]> GetFollowers(string id);
        Task<string[]> GetFollowing(string id);
    }
}
