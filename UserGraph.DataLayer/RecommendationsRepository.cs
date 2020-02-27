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
    // TODO: Or should recommendation endpoints all start with gathering the current user context
    // e.g. User Following Id's and Recent User Liked Posts.
    // This would allow us to query multiple recommendations at once

    /// <summary>
    /// Provides access to recommendation based queries for various vertices
    /// </summary>
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
            return await _g
                .V<User>(userId)
                .As((_, self) => _
                    .Out<Follows>()
                    .Aggregate((__, directFollows) => __
                        .Out<Follows>()
                            .OfType<User>()
                            .Where(x => x != self && !directFollows.Contains(x)))
                            .Dedup())
                .ToArrayAsync();
        }

        /// <summary>
        /// Finds users who like the same tweets as current user
        /// Excludes the current user and any users that the current is already following
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended users</returns>
        public async Task<User[]> GetUserRecommendationsBasedOnLikes(string userId)
        {
            return await _g
                .V<User>(userId)
                .Out<Follows>()
                .Fold()
                .As((___, directFollows) => ___
                    .V<User>(userId)
                    .Out<Likes>()
                    .OfType<Tweet>()
                    .In<Likes>()
                    .OfType<User>()
                    .Where(x => (string)x.Id != userId && !directFollows.Contains(x))
                    .Dedup())
                .ToArrayAsync();
        }

        /// <summary>
        /// Find tweets by users that the current user is following
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
        /// TODO: Find compatible users where they have "x" liked tweets in common and traverse those vertices edges
        /// </summary>
        /// <param name="userId">Current user's userId</param>
        /// <returns>List of recommended tweets</returns>
        public async Task<UserTweet[]> GetTweetRecommendationsBasedOnLikes(string userId)
        {
            // TODO: "recent tweets" should be within the same time frame to avoid duplicate tweets
            var dateTimeLimit = DateTime.Now.AddDays(-1);

            var recommendations = await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .Fold()
                .As((_, likedTweets) => _
                    .V<User>(userId)
                    .Out<Follows>()
                    .OfType<User>()
                    .As((__, followingUser) => __
                        .Out<Likes>()
                        .OfType<Tweet>()
                        .Where(x => !likedTweets.Contains(x))
                        .As((___, tweet) => ___
                            .Select(tweet, followingUser))))
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
