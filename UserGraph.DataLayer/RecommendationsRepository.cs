using System;
using System.Linq;
using System.Threading.Tasks;
using ExRam.Gremlinq.Core;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.DataLayer
{
    public class RecommendationsRepository : IRecommendationsRepository
    {
        private readonly IGremlinQuerySource _g;

        public RecommendationsRepository(IGremlinQuerySource g)
        {
            _g = g;
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

        /// <summary>
        /// Find tweets by users that thte current user is following
        /// Note: this is also the same as generating a basic Tweets timeline for a user
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended tweets</returns>
        public async Task<Tweet[]> GetTweetRecommendationsBasedOnFollows(string userId)
        {
            // TODO: Look into pagination in gremlin tinkerpop docs
            return await _g
                .V<User>(userId)
                .Out<Follows>()
                .Out<Created>()
                .OfType<Tweet>()
                .ToArrayAsync();
        }

        /// <summary>
        /// Finds tweets liked by users that the current user is following
        /// Excludes tweets already liked by the current user
        /// Tweets are limited to within the past 24 hours
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended tweets</returns>
        public async Task<Tweet[]> GetTweetRecommendationsBasedOnLikes(string userId)
        {
            // TODO: "recent tweets" should be within the same time frame to avoid duplicate tweets
            var dateTimeLimit = DateTime.Now.AddDays(-1);

            var likedTweets = await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .Values(x => x.Id)
                .ToArrayAsync();

            var followersLikeTweets = await _g
                .V<User>(userId)
                .Out<Follows>()
                .Out<Likes>()
                .OfType<Tweet>()
                .Where(x => !likedTweets.Contains(x.Id))
                .ToArrayAsync();

            return followersLikeTweets;
        }
    }
}
