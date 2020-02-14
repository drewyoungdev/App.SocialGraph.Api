using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Options;
using UserGraph.Configuration;
using UserGraph.DataLayer.Interfaces;

namespace UserGraph.DataLayer
{
    public class GremlinConnection : IGremlinConnection
    {
        public GremlinClient Client { get; internal set; }

        public GremlinConnection(IOptions<AzureSettings> options)
        {
            AzureSettings azureSettings = options.Value;

            Client = new GremlinClient(
                new GremlinServer(
                    azureSettings.GremlinEndpoint,
                    azureSettings.GremlinPort,
                    true,
                    azureSettings.GremlinDbCollection,
                    azureSettings.GremlinAuthKey),
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType);
        }
    }
}
