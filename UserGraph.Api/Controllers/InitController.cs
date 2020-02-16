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
            // Drop Graph
            await _g
                .V()
                .Drop()
                .ToArrayAsync();

            Random random = new Random();

            // Create Users & User Id List
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

            // Create Random Followers
            foreach (var user in users)
            {
                // Pick random int between 0 - NUMBER_OF_USERS
                int numberOfUsersFollowing = random.Next(0, NUMBER_OF_USERS);

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

            // TODO: Create Random Posts/Tweets
            // Pick random int between 0 - MAX_NUMBER_OF_POSTS
            // Add to Dictionary of UserIds -> Posts

            // TODO: Create Random Likes
            // foreach user loops through all users that's not the current one
            // Lookup user id in dictionary and pick random number of posts based on count to "like"

            return Ok();
        }
    }
}
