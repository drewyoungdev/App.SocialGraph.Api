using System;

namespace UserGraph.Models
{
    public class Tweet : Vertex
    {
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
