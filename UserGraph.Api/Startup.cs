using System;
using ExRam.Gremlinq.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserGraph.DataLayer;
using UserGraph.DataLayer.Interfaces;
using UserGraph.Models;

namespace UserGraph.Api
{
    // TODO: Add summaries
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// For local gremlin development:
        /// Install the Cosmos DB Local Emulator
        /// Start the emulator with the following command: CosmosDB.Emulator.exe /EnableGremlinEndpoint
        /// Create your database and your collection
        /// If you wish to connect to your gremlin server directly, follow these instructions:
        /// https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator#gremlin-api
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGremlinQuerySource>(serviceProvider =>
            {
                return GremlinQuerySource.g
                    //.AddStrategies(new PartitionKeyStrategy())
                    .ConfigureEnvironment(env => env
                       .UseModel(GraphModel.FromBaseTypes<Vertex, Edge>(lookup => lookup.IncludeAssembliesOfBaseTypes()))
                       .ConfigureModel(model => model
                           //.ConfigureElements(elem => elem.UseCamelCaseLabels())
                           .ConfigureProperties(prop => prop.UseCamelCaseNames())
                        )
                        .UseCosmosDb(
                            new Uri(Configuration["AzureSettings:GremlinEndpoint"]),
                            Configuration["AzureSettings:GremlinDatabaseName"],
                            Configuration["AzureSettings:GremlinCollectionName"],
                            Configuration["AzureSettings:GremlinAuthKey"])
                    );
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
