using Hydra.NET;

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
    /// Hydra context.
    /// </summary>
    protected Context HydraContext { get; }

    /// <summary>
    /// Resource base URI.
    /// </summary>
    protected Uri ResourceBaseUri { get; }
}