using ExRam.Gremlinq.Core;
using System;
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

        public async Task<User> CreateUser(User user)
        {
            if (user.Id == null)
                user.Id = Guid.NewGuid().ToString();

            return await _g
                .AddV(user)
                .FirstAsync();
        }

        public async Task<User[]> GetFollowers(string id)
        {
            return await _g
                .V<User>(id)
                .In<Follows>()
                .OfType<User>()
                // .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task<User[]> GetFollowing(string id)
        {
            return await _g
                .V<User>(id)
                .Out<Follows>()
                .OfType<User>()
                // .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task Follow(string sourceId, string destinationId)
        {
            await _g
                .V<User>(sourceId)
                .AddE<Follows>()
                .To(_ => _
                    .V<User>(destinationId))
                .FirstAsync();
        }

        // https://github.com/ExRam/ExRam.Gremlinq/issues/43
        public async Task Unfollow(string sourceId, string destinationId)
        {
            await _g
                .V<User>(sourceId)
                .OutE<Follows>()
                .As((_, e1) => _
                    .InV<User>()
                    .Where(user => (string)user.Id == destinationId)
                    .Select(e1))
                .Drop()
                .ToArrayAsync();
        }
    }
}
