using Asaph.Core.UseCases.AddSongDirector;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test boundary for the add song director use case which passes through the response.
/// </summary>
public class AddSongDirectorTestBoundary : IAddSongDirectorBoundary<AddSongDirectorResponse>
{
    /// <inheritdoc/>
    public AddSongDirectorResponse InsufficientPermissions(AddSongDirectorResponse response) =>
        response;

    /// <inheritdoc/>
    public AddSongDirectorResponse RequesterRankNotFound(AddSongDirectorResponse response) =>
        response;

    /// <inheritdoc/>
    public AddSongDirectorResponse SongDirectorAdded(AddSongDirectorResponse response) =>
        response;

    /// <inheritdoc/>
    public AddSongDirectorResponse SongDirectorAddFailed(AddSongDirectorResponse response) =>
        response;

    /// <inheritdoc/>
    public AddSongDirectorResponse ValidationFailure(AddSongDirectorResponse response) =>
        response;
}
