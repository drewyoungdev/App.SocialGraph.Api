using ExRam.Gremlinq.Core;
using System.Linq;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.DataLayer
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IGremlinQuerySource _g;

        public UsersRepository(IGremlinQuerySource g)
        {
            _g = g;
        }

        //await _gremlinQuerySource.V().Drop().ToArrayAsync();

        public async Task<User[]> GetAllUsers()
        {
            return await _g
                .V<User>()
                .ToArrayAsync();
        }

        public async Task<User> GetUser(string id)
        {
            return await _g
                .V<User>(id)
                .FirstAsync();
        }

        public async Task AddFollowsEdge(User source, User destination)
        {
            await _g
                .V<User>(source.Id)
                .AddE<Follows>()
                .To(_ => _
                    .V(destination.Id))
                .FirstAsync();
        }

        public async Task<string[]> GetFollowers(string id)
        {
            return await _g
                .V<User>(id)
                .In<Follows>()
                .OfType<User>()
                .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task<string[]> GetFollowing(string id)
        {
            return await _g
                .V<User>(id)
                .Out<Follows>()
                .OfType<User>()
                .Values(_ => _.Name)
                .ToArrayAsync();
        }
    }
}
