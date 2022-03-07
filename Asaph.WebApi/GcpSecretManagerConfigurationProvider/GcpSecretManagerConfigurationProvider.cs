using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;

namespace Asaph.WebApi.GcpSecretManagerConfigurationProvider;

/// <summary>
/// Configuration provider for GCP Secret Manager.
/// </summary>
public class GcpSecretManagerConfigurationProvider : ConfigurationProvider
{
    private readonly SecretManagerServiceClient _client;
    private readonly string _projectId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GcpSecretManagerConfigurationProvider"/> class.
    /// </summary>
    /// <param name="projectId">GCP project id.</param>
    /// <param name="secretManagerCredentialsPath">Secret Manager credentials path.</param>
    public GcpSecretManagerConfigurationProvider(
        string? projectId, string? secretManagerCredentialsPath)
    {
        if (projectId != null && secretManagerCredentialsPath != null)
        {
            SecretManagerServiceClientBuilder secretManagerServiceClientBuilder = new();
            secretManagerServiceClientBuilder.CredentialsPath = secretManagerCredentialsPath;
            _client = secretManagerServiceClientBuilder.Build();
        }
        else
        {
            _client = SecretManagerServiceClient.Create();
        }

        _projectId = string.IsNullOrWhiteSpace(projectId) ? GetGcpProjectId() : projectId;
    }

    /// <inheritdoc/>
    public override void Load()
    {
        IEnumerable<SecretName>? secretNames = _client
            .ListSecrets(new ProjectName(_projectId))?
            .Select(i => i.SecretName);

        if (secretNames?.Any() == false)
            return;

        Console.WriteLine($"Retrieved {secretNames!.Count()} secrets from Secret Manager.");

        foreach (SecretName secretName in secretNames!)
        {
            try
            {
                SecretVersionName secretVersionName = new(
                    secretName.ProjectId, secretName.SecretId, "latest");

                AccessSecretVersionResponse secretVersion = _client
                    .AccessSecretVersion(secretVersionName);

                Set(
                    NormalizeDelimiter(secretName.SecretId),
                    secretVersion.Payload.Data.ToStringUtf8());
            }
            catch (Grpc.Core.RpcException)
            {
                // Ignore. This might happen if the secret has no versions available.
            }
        }
    }

    /// <summary>
    /// Gets the GCP project id from the execution platform.
    /// </summary>
    /// <returns>Project id.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if GCP execution platform information couldn't be retrieved. This is most likely due
    /// to the service not running on GCP (e.g. local testing.)
    /// </exception>
    private static string GetGcpProjectId()
    {
        string? projectId = Platform.Instance()?.ProjectId;

        if (projectId == null)
        {
            throw new InvalidOperationException(
                "Could not retrieve GCP project id for GcpSecretManagerProvider.");
        }

        return projectId;
    }

    /// <summary>
    /// Normalizes the "__" (double underscore) key delimeter.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>The normalized key.</returns>
    private static string NormalizeDelimiter(string key)
    {
        return key.Replace("__", ConfigurationPath.KeyDelimiter);
    }
}