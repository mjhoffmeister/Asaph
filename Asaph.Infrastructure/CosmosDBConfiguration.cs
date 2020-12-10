namespace Asaph.Infrastructure
{
    public record CosmosDBConfiguration(string? ConnectionString, string? ContainerId, string? DatabaseId);
}
