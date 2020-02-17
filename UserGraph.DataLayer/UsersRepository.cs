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

        /// <summary>
        /// Finds user one-hop away from users the current user is following
        /// Excludes the current user and any users that the current is already following
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended users</returns>
        public async Task<User[]> GetUserRecommendationsBasedOnFollows(string userId)
        {
            // TODO: Figure out how to build into single query
            #region Test query
            // http://tinkerpop.apache.org/docs/current/recipes/#duplicate-vertex
            // g.V().has('id', 'Drew').as('self').out('Follows').aggregate('direct-follows').out('Follows').where(neq('self')).where(without('direct-follows'))

            // int limitFollowersOfFollowers = 10;

            // var query = await _g
            //     .V<User>(userId)
            //     .Out<Follows>()
            //     .OfType<User>()
            //     .Fold()
            //     .As((_, directFollows) => _
            //         .V<User>()
            //         .Where(user => directFollows.Contains(user)))
            //     .ToArrayAsync();

            // var query = await _g
            //     .V<User>(userId)
            //     .As((_, self) => _
            //         .Out<Follows>()
            //         .Aggregate((__, directFollows) => __
            //             .Out<Follows>()
            //                 .OfType<User>()
            //                 // How to do without directFollows
            //                 .Where(x => x != self
            //                     && !directFollows.Contains(x.Id)
            //                 ))
            //     )
            //     // .Cast<string>()
            //     // .FirstAsync();
            //     .OfType<User>()
            //     .ToArrayAsync();
            #endregion

            var directFollows = await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                .Values(x => x.Id)
                .ToArrayAsync();

            var recommendations = await _g
                .V<User>(userId)
                .Out<Follows>()
                .Out<Follows>()
                .OfType<User>()
                .Where(x => (string)x.Id != userId && !directFollows.Contains(x.Id))
                .ToArrayAsync();

            return recommendations;
        }

        /// <summary>
        /// Finds users who like the same tweets as current user
        /// Excludes the current user and any users that the current is already following
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended users</returns>
        public async Task<User[]> GetUserRecommendationsBasedOnLikes(string userId)
        {
            var directFollows = await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                .Values(x => x.Id)
                .ToArrayAsync();

            var recommendations = await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .In<Likes>()
                .OfType<User>()
                .Where(x => (string)x.Id != userId && !directFollows.Contains(x.Id))
                .Dedup()
                .ToArrayAsync();

            return recommendations;
        }
    }
}
