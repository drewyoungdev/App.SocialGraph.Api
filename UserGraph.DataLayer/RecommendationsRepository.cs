using System;
using System.Linq;
using System.Threading.Tasks;
using ExRam.Gremlinq.Core;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.DataLayer
{
    // TODO: Look into how to paginate queries so we can control of how far out a user can traverse our graph
    // But also have context of where they left off. This would essentially be the infinite scroll feature.
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

            // var result = await _g
            //     .V<User>(userId)
            //     .As((_, self) => _
            //         .Out<Follows>()
            //         .As((__, directFollower) => __
            //             .Out<Follows>()
            //                 .OfType<User>()
            //                 // How to exclude directFollows?
            //                 // .Where(x => x != self
            //                 //     && x != directFollower)
            //                 ))
            //     .Cast<string>()
            //     .FirstAsync();
                // .OfType<User>()
                // .ToArrayAsync();


            // Alternatively, could we just return all vertices and somehow group them and do the exclude in .NET
            var directFollowsAndTwoHopFollows = await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                .Aggregate((_, directFollows) => _
                    .Out<Follows>()
                    .OfType<User>()
                    .Aggregate((__, twoHopFollows) => __
                        .Select(directFollows, twoHopFollows)))
                .Dedup()
                .FirstOrDefaultAsync();

            if (directFollowsAndTwoHopFollows.Item1 == null || directFollowsAndTwoHopFollows.Item2 == null)
                return new User[] {};

            var recommendations = directFollowsAndTwoHopFollows.Item2
                .Where(x => (string)x.Id != userId)
                .Except(directFollowsAndTwoHopFollows.Item1, new UserVertextComparer())
                .ToArray();

            return recommendations;
            #endregion

            // var directFollows = await _g
            //     .V<User>(userId)
            //     .Out<Follows>()
            //     .OfType<User>()
            //     .Values(x => x.Id)
            //     .ToArrayAsync();

            // var recommendations = await _g
            //     .V<User>(userId)
            //     .Out<Follows>()
            //     .Out<Follows>()
            //     .OfType<User>()
            //     .Where(x => (string)x.Id != userId && !directFollows.Contains(x.Id))
            //     .Dedup()
            //     .ToArrayAsync();

            // return recommendations;
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
        public async Task<UserTweet[]> GetTweetRecommendationsBasedOnFollows(string userId)
        {
            // TODO: Look into pagination in gremlin tinkerpop docs
            (Tweet, User)[] recommendations = await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                .As((_, followingUser) => _
                    .Out<Created>()
                    .OfType<Tweet>()
                    .As((__, tweet) => __
                        .Select(tweet, followingUser)))
                .ToArrayAsync();

            return recommendations
                .Select(x => new UserTweet()
                {
                    Tweet = x.Item1,
                    CreatedByUser = x.Item2
                })
                .ToArray();
        }

        /// <summary>
        /// Finds tweets liked by users that the current user is following
        /// Excludes tweets already liked by the current user
        /// Tweets are limited to within the past 24 hours
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended tweets</returns>
        public async Task<UserTweet[]> GetTweetRecommendationsBasedOnLikes(string userId)
        {
            // TODO: "recent tweets" should be within the same time frame to avoid duplicate tweets
            var dateTimeLimit = DateTime.Now.AddDays(-1);

            var likedTweets = await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .Values(x => x.Id)
                .ToArrayAsync();

            (Tweet, User)[] recommendations = await _g
                .V<User>(userId)
                .Out<Follows>()
                .OfType<User>()
                .As((_, followingUser) => _
                    .Out<Likes>()
                    .OfType<Tweet>()
                    .Where(x => !likedTweets.Contains(x.Id))
                    .As((__, tweet) => __
                        .Select(tweet, followingUser)))
                .ToArrayAsync();

            return recommendations
                .Select(x => new UserTweet()
                {
                    Tweet = x.Item1,
                    CreatedByUser = x.Item2
                })
                .ToArray();
        }
    }
}
