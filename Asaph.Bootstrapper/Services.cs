using Asaph.Core.Interfaces;
using Asaph.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asaph.Bootstrapper
{
    public static class Services
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            // Get configuration for Cosmos DB
            var cosmosDBConfiguration = configuration.GetSection("cosmosDBConfiguration").Get<CosmosDBConfiguration>();

            // Configure services
            services.AddTransient<IAsyncSongDirectorRepository, CosmosDBSongDirectorRepository>(_ => new(cosmosDBConfiguration));
        }
    }
}
