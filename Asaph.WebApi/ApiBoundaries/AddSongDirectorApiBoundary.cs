using Asaph.Core.UseCases.AddSongDirector;
using Hydra.NET;
using System.Net;

/// <summary>
/// API boundary for the Add Song Director use case.
/// </summary>
internal class AddSongDirectorApiBoundary : ApiBoundary, IAddSongDirectorBoundary<IResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSongDirectorApiBoundary"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public AddSongDirectorApiBoundary(ApiBoundaryConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc/>
    public IResult InsufficientPermissions(AddSongDirectorResponse response) =>
        Results.Unauthorized();

    /// <inheritdoc/>
    /// TODO: Create an UnauthorizedObjectResult class to include a Status object
    public IResult RequesterRankNotFound(AddSongDirectorResponse response) =>
        Results.Unauthorized();

    /// <inheritdoc/>
    public IResult SongDirectorAdded(AddSongDirectorResponse response)
    {
        // Validate song director id
        if (response.AddedSongDirector?.Id == null)
            throw new ArgumentException("Id must be set for an added song director.");

        // Get the URL for the added song director
        string addedSongDirectorUrl = new Uri(
            ResourceBaseUri, response.AddedSongDirector.Id)
            .ToString();

        // Create a response object for the added song director
        SongDirectorApiModel addSongDirectorResponse = SongDirectorApiModel
            .AddedSongDirector(addedSongDirectorUrl);

        // Return the IActionResult representing the add
        return Results.Created(addedSongDirectorUrl, addSongDirectorResponse);
    }

    /// <inheritdoc/>
    public IResult SongDirectorAddFailed(AddSongDirectorResponse response) =>
        Results.Extensions.BadGatewayStatusJsonLD(HydraContext, response.Messages);

    /// <inheritdoc/>
    public IResult ValidationFailure(AddSongDirectorResponse response) =>
        Results.BadRequest(new Status(
            HydraContext,
            (int)HttpStatusCode.BadRequest,
            "Bad Request",
            string.Join(Environment.NewLine, response.Messages)));
}