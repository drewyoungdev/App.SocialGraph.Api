namespace UserGraph.Configuration
{
    public class AzureSettings
    {
        public string GremlinEndpoint { get; set; }
        public int GremlinPort { get; set; }
        public string GremlinDbCollection { get; set; }
        public string GremlinAuthKey { get; set; }
    }
}
