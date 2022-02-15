namespace Asaph.WebApi.GcpSecretManagerConfigurationProvider;

/// <summary>
/// Provides extensions for adding GCP Secret Manager secrets to configuration.
/// </summary>
public static class GcpSecretManagerConfigurationExtensions
{
    /// <summary>
    /// Adds GCP Secret Manager as a configuration source.
    /// </summary>
    /// <param name="builder">Configuration builder.</param>
    /// <param name="configuration">Configuration. "Gcp" is the assumed section.</param>
    /// <returns>The updated configuration builder.</returns>
    public static IConfigurationBuilder AddGcpSecretManager(
        this IConfigurationBuilder builder, IConfiguration configuration)
    {
        string? projectId = configuration["Gcp:ProjectId"];
        string? secretManagerCredentialsPath = configuration["Gcp:SecretManagerCredentialsPath"];

        builder.Add(new GcpSecretManagerConfigurationSource(
            projectId, secretManagerCredentialsPath));

        return builder;
    }
}
