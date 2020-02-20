using System.Collections.Generic;

namespace UserGraph.Models
{
    public class UserVertextComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            return x.Id.ToString() == y.Id.ToString();
        }

        public int GetHashCode(User user)
        {
            return user.Id.ToString().GetHashCode();
        }
    }
}
