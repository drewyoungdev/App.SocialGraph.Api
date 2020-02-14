using Gremlin.Net.Driver;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IGremlinConnection
    {
        GremlinClient Client { get; }
    }
}
