namespace UserGraph.Models
{
    public class Likes : Edge
    {
        // TODO: track like date https://github.com/ExRam/ExRam.Gremlinq/issues/6
        // We can then pull all likes before "x" datetime.
        // This allows client to query tweets created before "x" datetime too match if tweet is liked by user already
    }
}
