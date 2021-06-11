namespace Asaph.Infrastructure
{
    public record CosmosDBConfiguration
    {
        public CosmosDBConfiguration() => (ConnectionString, ContainerId, DatabaseId) = 
            (null, null, null);

        public CosmosDBConfiguration(
            string? connectionString, string? containerId, string databaseId) =>
            (ConnectionString, ContainerId, DatabaseId) = 
                (connectionString, containerId, databaseId);

        public string? ConnectionString { get; init; }

        public string? ContainerId { get; init; }

        public string? DatabaseId { get; init; }
    }
}
