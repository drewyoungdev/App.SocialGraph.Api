using System;

namespace UserGraph.Models
{
    public class UserTweet
    {
        public Tweet Tweet { get; set; }
        public User CreatedByUser { get; set; }
    }
}
