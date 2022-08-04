using Hydra.NET;
using System.Net;

/// <summary>
/// Results extenstions.
/// </summary>
internal static class ResultsExtensions
{
    /// <summary>
    /// Returns a Bad Gateway response with a Hydra Status object serialized as JSON LD.
    /// </summary>
    /// <param name="resultExtensions"><see cref="IResultExtensions"/>.</param>
    /// <param name="hydraContext">Hydra context.</param>
    /// <param name="message">Error message.</param>
    /// <returns><see cref="IResult"/>.</returns>
    public static IResult BadGatewayStatusJsonLD(
        this IResultExtensions resultExtensions,
        Context hydraContext,
        string message)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new BadGatewayObjectResult(
            new Status(
                hydraContext,
                (int)HttpStatusCode.BadGateway,
                "Bad Gateway",
                message),
            "application/ld+json");
    }

    /// <summary>
    /// Returns a Bad Gateway response with a Hydra Status object serialized as JSON LD.
    /// </summary>
    /// <param name="resultExtensions"><see cref="IResultExtensions"/>.</param>
    /// <param name="hydraContext">Hydra context.</param>
    /// <param name="messages">Error messages.</param>
    /// <returns><see cref="IResult"/>.</returns>
    public static IResult BadGatewayStatusJsonLD(
        this IResultExtensions resultExtensions,
        Context hydraContext,
        IEnumerable<string> messages)
    {
        return resultExtensions.BadGatewayStatusJsonLD(
            hydraContext, string.Join(Environment.NewLine, messages));
    }
}