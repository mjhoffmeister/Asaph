using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Infrastructure.SongDirectorRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asaph.Bootstrapper
{
    /// <summary>
    /// Utility class for configuring services.
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// Configures services for a given service collection and configuration.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="configuration">Configuration.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAsaphServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Get Azure AD B2C configuration
            AzureAdb2cConfiguration azureAdb2CConfiguration = configuration
                .GetSection("AzureAdb2c")
                .Get<AzureAdb2cConfiguration>();

            System.Console.WriteLine($"Client id: {azureAdb2CConfiguration.ClientId}.");
            System.Console.WriteLine($"Extensions client id: {azureAdb2CConfiguration.ExtensionsAppClientId}.");

            // Get Dynamo DB configuration
            DynamoDBConfiguration dynamoDBConfiguration = configuration
                .GetSection("DynamoDB")
                .Get<DynamoDBConfiguration>();

            // Register services
            services.AddTransient<
                        ISongDirectorRepositoryFragment,
                        AzureAdb2cSongDirectorRepository>(
                            factory => new(azureAdb2CConfiguration))
                    .AddTransient<
                        ISongDirectorRepositoryFragment,
                        DynamoDBSongDirectorRepository>(
                            factory => new(dynamoDBConfiguration))
                    .AddTransient<
                        IAsyncRepository<SongDirector>,
                        AggregateSongDirectorRepository>();

            // Return the service collection reference
            return services;
        }
    }
}
