using ExRam.Gremlinq.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserGraph.Configuration;
using UserGraph.DataLayer;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api
{
    // TODO: Remove unnecessary projects
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureSettings>(Configuration.GetSection(nameof(AzureSettings)));

            string gremlinUri = Configuration["AzureSettings:GremlinEndpoint"];
            string gremlinDatabaseName = Configuration["AzureSettings:GremlinDatabaseName"];
            string gremlinCollectionName = Configuration["AzureSettings:GremlinCollectionName"];
            string gremlinAuthKey = Configuration["AzureSettings:GremlinAuthKey"];

            services.AddSingleton<IGremlinQuerySource>(serviceProvider =>
            {
                return GremlinQuerySource.g
                    //.AddStrategies(new PartitionKeyStrategy())
                    // for 7.x.x
                    .UseModel(GraphModel.FromBaseTypes<Vertex, Edge>())
                    .ConfigureModel(model => model
                        //.ConfigureElements(elem => elem.UseCamelCaseLabels())
                        .ConfigureProperties(prop => prop.UseCamelCaseNames())
                    )
                    //.UseLogger(logger)
                    // for 8.x.x preview:
                    //.ConfigureEnvironment(env => env
                    //    .UseModel(GraphModel.FromBaseTypes<Vertex, Edge>(lookup => lookup.IncludeAssembliesOfBaseTypes()))
                    //    .ConfigureModel(model => model
                    //        //.ConfigureElements(elem => elem.UseCamelCaseLabels())
                    //        .ConfigureProperties(prop => prop.UseCamelCaseNames()))
                    //    )
                    //    //.UseExecutionPipeline(GremlinQueryExecutionPipeline.EchoGroovy)
                    //    //.UseLogger(logger)
                    // // For returning generated query
                    // .UseExecutionPipeline(GremlinQueryExecutionPipeline.EchoGroovyString)
                    .UseCosmosDb(gremlinUri, gremlinDatabaseName, gremlinCollectionName, gremlinAuthKey)
                    ;
            });

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITweetsRepository, TweetsRepository>();
            services.AddScoped<IRecommendationsRepository, RecommendationsRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json",
                            optional: false,
                            reloadOnChange: true)
               .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                builder.AddUserSecrets<Startup>();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    // add a partition key strategy
    // https://medium.com/@jayanta.mondal/getting-the-best-out-of-cosmos-dbs-scale-out-graph-api-aka-partitioned-graph-containers-bf47c240e698
    //public class PartitionKeyStrategy : IGremlinQueryStrategy
    //{
    //    public IGremlinQueryBase Apply(IGremlinQueryBase query)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
