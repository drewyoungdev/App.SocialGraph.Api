using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IGremlinPersistence
    {
        Task<IReadOnlyCollection<dynamic>> SubmitWithParametersAsync<TDynamic>(string query, Dictionary<string, object> parameters);
    }
}
