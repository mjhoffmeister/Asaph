using Asaph.Core.UseCases.RemoveSongDirector;
using Hydra.NET;
using System.Net;

namespace Asaph.WebApi.ApiBoundaries;

/// <summary>
/// API boundary for the Remove Song Director use case.
/// </summary>
internal class RemoveSongDirectorApiBoundary : ApiBoundary, IRemoveSongDirectorBoundary<IResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveSongDirectorApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public RemoveSongDirectorApiBoundary(ApiBoundaryConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc/>
    public IResult CannotRemoveSelf(RemoveSongDirectorResponse response) =>
        Results.BadRequest(new Status(
            HydraContext,
            (int)HttpStatusCode.BadRequest,
            "Bad Request",
            response.Message));

    /// <inheritdoc/>
    public IResult InsufficientPermissions(RemoveSongDirectorResponse response) =>
        Results.Unauthorized();

    /// <inheritdoc/>
    public IResult RemovalFailed(RemoveSongDirectorResponse response) =>
        Results.Extensions.BadGatewayStatusJsonLD(HydraContext, response.Message);

    /// <inheritdoc/>
    public IResult SongDirectorRemoved(RemoveSongDirectorResponse response) =>
        Results.Ok(new Status(
            HydraContext,
            (int)HttpStatusCode.OK,
            "OK",
            response.Message));
}
