using Hydra.NET;
using System.Configuration;

internal record ApiBoundaryConfiguration(
    string HydraContextUriString, string ResourceBaseUriString);

/// <summary>
/// API boundary base class.
/// </summary>
internal abstract class ApiBoundary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    protected ApiBoundary(ApiBoundaryConfiguration configuration)
    {
        HydraContext = new(new Uri(configuration.HydraContextUriString));
        ResourceBaseUri = new(configuration.ResourceBaseUriString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="relativeResourceUrl">Relative resource URL.</param>
    protected ApiBoundary(IConfiguration configuration, string relativeResourceUrl)
    {
        string? hydraContextUri = configuration["HydraContextUri"];

        if (hydraContextUri == null)
            throw new ConfigurationErrorsException("Missing Hydra context URI configuration.");

        string? baseUri = configuration["BaseUri"];

        if (baseUri == null)
            throw new ConfigurationErrorsException("Missing base URI configuration.");

        HydraContext = new Context(new Uri(hydraContextUri));
        ResourceBaseUri = new Uri($"{baseUri.TrimEnd('/')}{relativeResourceUrl}/");
    }

    /// <summary>
    /// Hydra context.
    /// </summary>
    protected Context HydraContext { get; }

    /// <summary>
    /// Resource base URI.
    /// </summary>
    protected Uri ResourceBaseUri { get; }
}