namespace Asaph.WebApi.GcpSecretManagerConfigurationProvider;

/// <summary>
/// GCP Secret Manager configurtion source.
/// </summary>
public class GcpSecretManagerConfigurationSource : IConfigurationSource
{
    // Project id
    private readonly string? _projectId;

    // Secret Manager credentials path
    private readonly string? _secretManagerCredentialsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="GcpSecretManagerConfigurationSource"/> class.
    /// </summary>
    /// <param name="projectId">Project id.</param>
    /// <param name="secretManagerCredentialsPath">Secret Manager credentials path.</param>
    public GcpSecretManagerConfigurationSource(
        string? projectId, string? secretManagerCredentialsPath)
    {
        _projectId = projectId;
        _secretManagerCredentialsPath = secretManagerCredentialsPath;
    }

    /// <inheritdoc/>
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new GcpSecretManagerConfigurationProvider(_projectId, _secretManagerCredentialsPath);
}