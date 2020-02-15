using ExRam.Gremlinq.Core.GraphElements;

namespace UserGraph.Models
{
    public class Edge : IEdge
    {
        public object Id { get; set; }
        public string Label { get; set; }
    }
}
