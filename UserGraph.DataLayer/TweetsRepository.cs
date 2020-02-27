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
                .Out<Created>()
                .OfType<Tweet>()
                .ToArrayAsync();
        }

        public async Task<UserTweet[]> GetTweetsLikedByUserId(string userId)
        {
            (Tweet, User)[] userAndTweets = await _g
                .V<User>(userId)
                .Out<Likes>()
                .OfType<Tweet>()
                .As((_, tweet) => _
                    .In<Created>()
                    .OfType<User>()
                    .As((__, createdByUser) => __
                        .Select(tweet, createdByUser)))
                .ToArrayAsync();

            return userAndTweets
                .Select(x => new UserTweet()
                {
                    Tweet = x.Item1,
                    CreatedByUser = x.Item2
                })
                .ToArray();
        }

        public async Task Tweet(string userId, Tweet tweet)
        {
            if (tweet.Id == null)
            {
                tweet.Id = Guid.NewGuid().ToString();
                tweet.CreatedDate = DateTime.UtcNow;
            }

            await _g
                .V<User>(userId)
                .AddE<Created>()
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

        public async Task<long> GetLikesCount(string tweetId)
        {
            return await _g
                .V<Tweet>(tweetId)
                .In<Likes>()
                .Count()
                .FirstAsync();
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
    }
}
