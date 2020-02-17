using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExRam.Gremlinq.Core;
using Microsoft.AspNetCore.Mvc;
using UserGraph.Models;

namespace UserGraph.Api.Controllers
{
    [Route("api/init")]
    public class InitController : ControllerBase
    {
        private const int NUMBER_OF_USERS = 5;
        private const int NUMBER_OF_TWEETS_PER_USER = 10;

        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string RandomString(Random random)
        {
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private readonly IGremlinQuerySource _g;

        public InitController(IGremlinQuerySource g)
        {
            _g = g;
        }

        [HttpPost]
        public async Task<IActionResult> Init()
        {
            await DropTables();

            Random random = new Random();

            var users = await CreateNonRandomUsers();
            // List<User> users = await CreateRandomUsers(random);

            await CreateRandomFollowers(users, random);

            var userIdsAndPosts = await CreateRandomTweets(users, random);

            await CreateRandomLikes(users, userIdsAndPosts, random);

            return Ok();
        }

        private async Task DropTables()
        {
            await _g
                .E()
                .Drop()
                .ToArrayAsync();

            await _g
                .V()
                .Drop()
                .ToArrayAsync();
        }

        private async Task<List<User>> CreateNonRandomUsers()
        {
            var users = new List<User>()
            {
                new User(){ Id = "Drew", Name = "Drew" },
                new User(){ Id = "Mike", Name = "Mike" },
                new User(){ Id = "Andre", Name = "Andre"},
                new User(){ Id = "Jon", Name = "Jon" },
                new User(){ Id = "Erica", Name = "Erica" }
            };

            foreach (var user in users)
            {
                await _g
                    .AddV(user)
                    .FirstAsync();
            }

            return users;
        }

        private async Task<List<User>> CreateRandomUsers(Random random)
        {
            List<User> users = new List<User>(NUMBER_OF_USERS);

            for (int i = 0; i < NUMBER_OF_USERS; i++)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = RandomString(random)
                };

                await _g
                    .AddV(user)
                    .FirstAsync();

                users.Add(user);
            }

            return users;
        }

        private async Task CreateRandomFollowers(List<User> users, Random random)
        {
            foreach (var user in users)
            {
                // Pick random int between 0 - NUMBER_OF_USERS
                int numberOfUsersFollowing = random.Next(0, users.Count + 1);

                // Create list of "following" list
                var usersFollowing = new List<User>(numberOfUsersFollowing);

                for (int i = 0; i < numberOfUsersFollowing; i++)
                {
                    if (user.Id != users[i].Id)
                        usersFollowing.Add(users[i]);
                }

                // Create outbound edges for all users in "following" list for current user
                foreach (var userFollowing in usersFollowing)
                {
                    await _g
                        .V<User>(user.Id)
                        .AddE<Follows>()
                        .To(_ => _
                            .V<User>(userFollowing.Id))
                        .FirstAsync();
                }
            }
        }

        private async Task<Dictionary<string, List<Tweet>>> CreateRandomTweets(List<User> users, Random random)
        {
            var userIdsAndPosts = new Dictionary<string, List<Tweet>>();

            foreach (var user in users)
            {
                // Pick random int between 0 - MAX_NUMBER_OF_TWEETS
                int numberOfTweets = random.Next(0, NUMBER_OF_TWEETS_PER_USER + 1);

                var tweets = new List<Tweet>(numberOfTweets);

                for (int i = 0; i < numberOfTweets; i++)
                {
                    var tweet = new Tweet()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = RandomString(random),
                        CreatedDate = DateTime.UtcNow
                    };

                    await _g
                        .V<User>(user.Id)
                        .AddE<Created>()
                        .To(_ => _
                            .AddV(tweet))
                        .FirstAsync();

                    tweets.Add(tweet);
                }

                userIdsAndPosts.Add((string)user.Id, tweets);
            }

            return userIdsAndPosts;
        }

        private async Task CreateRandomLikes(List<User> users, Dictionary<string, List<Tweet>> userIdsAndPosts, Random random)
        {
            foreach (var user in users)
            {
                // foreach user loops through all users that's not the current one
                foreach (var userId in userIdsAndPosts.Keys)
                {
                    if (userId != (string)user.Id)
                    {
                        List<Tweet> tweets = userIdsAndPosts[userId];

                        // Lookup user id in dictionary and pick random number of posts based on count to "like"
                        int numberOfLikes = random.Next(0, tweets.Count + 1);

                        for (int i = 0; i < numberOfLikes; i++)
                        {
                            await _g
                                .V<User>(user.Id)
                                .AddE<Likes>()
                                .To(_ => _
                                    .V<Tweet>(tweets[i].Id))
                                .FirstAsync();
                        }
                    }
                }
            }
        }
    }
}
