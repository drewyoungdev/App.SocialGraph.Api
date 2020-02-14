using System.Threading.Tasks;

namespace UserGraph.DataLayer.Interfaces
{
    public interface IGremlinRepository
    {
        Task<T> AddVertex<T>();
    }
}
