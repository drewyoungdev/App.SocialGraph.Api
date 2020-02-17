using ExRam.Gremlinq.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.DataLayer
{
    // TODO: Vertices have similar pattern. This may call for a BaseRepository for GetAll<T>, GetById<T>. Same with Edges
    public class UsersRepository : IUsersRepository
    {
        private readonly IGremlinQuerySource _g;

        public UsersRepository(IGremlinQuerySource g)
        {
            _g = g;
        }

        public async Task<User[]> GetAllUsers()
        {
            return await _g
                .V<User>()
                .ToArrayAsync();
        }

        public async Task<User> GetUserById(string id)
        {
            return await _g
                .V<User>(id)
                .FirstOrDefaultAsync();
        }

        public async Task<User> CreateUser(User user)
        {
            if (user.Id == null)
                user.Id = Guid.NewGuid().ToString();

            return await _g
                .AddV(user)
                .FirstAsync();
        }

        public async Task<User[]> GetFollowers(string userId)
        {
            return await _g
                .V<User>(userId)
                .In<Follows>()
                .OfType<User>()
                // .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task<User[]> GetFollowing(string userId)
        {
            return await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                // .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task Follow(string sourceUserId, string destinationUserId)
        {
            await _g
                .V<User>(sourceUserId)
                .AddE<Follows>()
                .To(_ => _
                    .V<User>(destinationUserId))
                .FirstAsync();
        }

        // https://github.com/ExRam/ExRam.Gremlinq/issues/43
        public async Task Unfollow(string sourceUserId, string destinationUserId)
        {
            await _g
                .V<User>(sourceUserId)
                .OutE<Follows>()
                .As((_, e1) => _
                    .InV<User>()
                    .Where(user => (string)user.Id == destinationUserId)
                    .Select(e1))
                .Drop()
                .ToArrayAsync();
        }
    }
}
