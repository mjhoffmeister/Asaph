using Asaph.Core.UseCases.UpdateSongDirector;
using Hydra.NET;
using System.Net;

namespace Asaph.WebApi.UseCases.UpdateSongDirector;

/// <summary>
/// API boundary for the Update Song Director use case.
/// </summary>
internal class UpdateSongDirectorApiBoundary : ApiBoundary, IUpdateSongDirectorBoundary<IResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSongDirectorApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public UpdateSongDirectorApiBoundary(IConfiguration configuration)
        : base(configuration, RelativeResourceUrls.SongDirectors)
    {
    }

    /// <inheritdoc/>
    public IResult InsufficientPermissions(UpdateSongDirectorResponse response)
    {
        return Results.Unauthorized();
    }

    /// <inheritdoc/>
    public IResult InvalidRequest(UpdateSongDirectorResponse response)
    {
        return Results.BadRequest(new Status(
            HydraContext,
            (int)HttpStatusCode.BadRequest,
            "Bad Request",
            response.Message));
    }

    /// <inheritdoc/>
    public IResult RequesterRankNotFound(UpdateSongDirectorResponse response)
    {
        return Results.Extensions.ForbiddenStatusJsonLD(HydraContext, response.Message);
    }

    /// <inheritdoc/>
    public IResult SongDirectorUpdated(UpdateSongDirectorResponse response)
    {
        return Results.Ok(new Status(
            HydraContext,
            (int)HttpStatusCode.OK,
            "OK",
            response.Message));
    }

    /// <inheritdoc/>
    public IResult SongDirectorUpdateFailed(UpdateSongDirectorResponse response)
    {
        return Results.Extensions.BadGatewayStatusJsonLD(HydraContext, response.Message);
    }
}