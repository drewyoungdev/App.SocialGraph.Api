using System;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;

namespace UserGraph.DataLayer
{
    public class GremlinRepository : IGremlinRepository
    {
        private IGremlinConnection _gremlinConnection;

        public GremlinRepository(IGremlinConnection gremlinConnection)
        {
            _gremlinConnection = gremlinConnection;
        }

        public Task<T> AddVertex<T>()
        {
            throw new NotImplementedException();
        }
    }
}
