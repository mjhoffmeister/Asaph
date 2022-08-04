using Asaph.Core.UseCases.GetSongDirectors;
using Asaph.WebApi.UseCases;
using Hydra.NET;
using System.Net;

/// <summary>
/// API boundary for the Get Song Directors use case.
/// </summary>
internal class GetSongDirectorsApiBoundary : ApiBoundary, IGetSongDirectorsBoundary<IResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSongDirectorsApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public GetSongDirectorsApiBoundary(IConfiguration configuration)
        : base(configuration, RelativeResourceUrls.SongDirectors)
    {
    }

    /// <inheritdoc/>
    public IResult FailedToGetSongDirectors(GetSongDirectorsResponse response) =>
        new BadGatewayObjectResult(
            new Status(
            HydraContext,
            (int)HttpStatusCode.BadGateway,
            "Bad Gateway",
            response.Message),
            "application/ld+json");

    /// <inheritdoc/>
    public IResult InvalidRequesterEmailAddress(GetSongDirectorsResponse response) =>
        Results.BadRequest(new Status(
            HydraContext,
            (int)HttpStatusCode.BadRequest,
            "Bad Request",
            response.Message));

    /// <inheritdoc/>
    // TODO: Create UnauthorizedObjectResult
    public IResult RequesterSongDirectorRankNotFound(GetSongDirectorsResponse response) =>
        Results.Unauthorized();

    /// <inheritdoc/>
    public IResult Success(GetSongDirectorsResponse response)
    {
        if (response.SongDirectors == null)
        {
            throw new ArgumentException("Song directors must be set for a successful Get" +
                "Song Directors response.");
        }

        // Convert use case models to API models
        IEnumerable<SongDirectorApiModel> songDirectorApiModels = response.SongDirectors
            .Select(useCaseModel => SongDirectorApiModel
                .RetrievedSongDirector(
                    HydraContext,
                    new Uri(ResourceBaseUri, useCaseModel.Id),
                    useCaseModel));

        return Results.Ok(songDirectorApiModels);
    }
}