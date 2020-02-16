using ExRam.Gremlinq.Core.GraphElements;

namespace UserGraph.Models
{
    public class Vertex : IVertex
    {
        public object Id { get; set; }
        public string Label { get; set; }
        public string Type => GetType().Name;
        public string PartitionKey => $"{Type}_{Id}";
    }
}
