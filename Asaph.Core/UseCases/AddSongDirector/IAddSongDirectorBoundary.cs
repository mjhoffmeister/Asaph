namespace Asaph.Core.UseCases.AddSongDirector;

/// <summary>
/// Boundary interface for the Add Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public interface IAddSongDirectorBoundary<TOutput> : IBoundary<AddSongDirectorResponse, TOutput>
{
    /// <summary>
    /// Insufficient permissions.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput InsufficientPermissions(AddSongDirectorResponse response);

    /// <summary>
    /// Requester rank not found.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput RequesterRankNotFound(AddSongDirectorResponse response);

    /// <summary>
    /// Song director successfully added.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput SongDirectorAdded(AddSongDirectorResponse response);

    /// <summary>
    /// Song director add failed.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput SongDirectorAddFailed(AddSongDirectorResponse response);

    /// <summary>
    /// Validation failure.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput ValidationFailure(AddSongDirectorResponse response);
}