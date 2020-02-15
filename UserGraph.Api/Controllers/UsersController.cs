using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Core.GraphElements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UserGraph.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IGremlinQuerySource _gremlinQuerySource;

        public UsersController(IGremlinQuerySource gremlinQuerySource)
        {
            _gremlinQuerySource = gremlinQuerySource;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _gremlinQuerySource.V().Drop();

            var person1 = new Person()
            {
                Name = new VertexProperty<string, NameMeta>("Drew")
                    {
                        Properties = new NameMeta()
                        {
                            Creator = "User1",
                            Date = DateTime.Now
                        }
                    },
                Age = 125
            };

            var person2 = new Person()
            {
                Name = new VertexProperty<string, NameMeta>("Erica")
                {
                    Properties = new NameMeta()
                    {
                        Creator = "User2",
                        Date = DateTime.Now
                    }
                },
                Age = 126
            };

            var result = await _gremlinQuerySource
                .AddV(person1)
                .AddE<Knows>()
                    .To(x => x.AddV(person2))
                .FirstAsync();

            return Ok(result);
        }

        [HttpGet("{id}/knows")]
        public async Task<IActionResult> Knows(string id)
        {
            Person[] knows = await _gremlinQuerySource
                .V(id)
                .Out<Knows>()
                .OfType<Person>()
                .ToArrayAsync();

            return Ok(knows);
        }
    }

    public class Person : Vertex
    {
        public int Age { get; set; }
        public VertexProperty<string, NameMeta> Name { get; set; }
    }

    public class NameMeta
    {
        public string Creator { get; set; }
        public DateTime Date { get; set; }
    }

    public class Vertex : IVertex
    {
        public object Id { get; set; }
        public string Label { get; set; }
        public string PartitionKey { get; set; } = "PartitionKey";
    }

    public class Knows : Edge
    { }

    public class Edge : IEdge
    {
        public object Id { get; set; }
        public string Label { get; set; }
    }
}
