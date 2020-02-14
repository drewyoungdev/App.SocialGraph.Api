using Gremlin.Net.Process.Traversal;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserGraph.DataLayer.Interfaces;

namespace UserGraph.DataLayer
{
    public class GremlinPersistence : IGremlinPersistence
    {
        public Task<IReadOnlyCollection<dynamic>> SubmitWithParametersAsync<TDynamic>(string query, Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
