using ExRam.Gremlinq.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.DataLayer
{
    public class TweetsRepository : ITweetsRepository
    {
        private readonly IGremlinQuerySource _g;

        public TweetsRepository(IGremlinQuerySource g)
        {
            _g = g;
        }

        public async Task<Tweet[]> GetAllTweets()
        {
            return await _g
                .V<Tweet>()
                .ToArrayAsync();
        }

        public async Task<Tweet> GetTweetById(string tweetId)
        {
            return await _g
                .V<Tweet>(tweetId)
                .FirstOrDefaultAsync();
        }

        public async Task<Tweet[]> GetTweetsByUserId(string userId)
        {
            return await _g
                .V<User>(userId)
                .Out<CreatedBy>()
                .OfType<Tweet>()
                .ToArrayAsync();
        }

        public async Task<Tweet[]> GetTweetsLikedByUserId(string userId)
        {
            return await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .ToArrayAsync();
        }

        public async Task Tweet(string userId, Tweet tweet)
        {
            if (tweet.Id == null)
                tweet.Id = Guid.NewGuid().ToString();

            await _g
                .V<User>(userId)
                .AddE<CreatedBy>()
                .To(_ => _
                    .AddV(tweet))
                .FirstAsync();
        }

        public async Task<User[]> GetLikes(string tweetId)
        {
            return await _g
                .V<Tweet>(tweetId)
                .In<Likes>()
                .OfType<User>()
                // .Values(_ => _.Name)
                .ToArrayAsync();
        }

        public async Task<int> GetLikesCount(string tweetId)
        {
            return await _g
                .V<Tweet>(tweetId)
                .In<Likes>()
                .CountAsync();
        }

        public async Task Like(string sourceUserId, string destinationTweetId)
        {
            await _g
                .V<User>(sourceUserId)
                .AddE<Likes>()
                .To(_ => _
                    .V<Tweet>(destinationTweetId))
                .FirstAsync();
        }

        public async Task Unlike(string sourceUserId, string destinationTweetId)
        {
            await _g
                .V<User>(sourceUserId)
                .OutE<Likes>()
                .As((_, e1) => _
                    .InV<Tweet>()
                    .Where(tweet => (string)tweet.Id == destinationTweetId)
                    .Select(e1))
                .Drop()
                .ToArrayAsync();
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
                .Out<CreatedBy>()
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
