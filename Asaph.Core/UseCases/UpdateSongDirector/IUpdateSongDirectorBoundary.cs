namespace Asaph.Core.UseCases.UpdateSongDirector;

/// <summary>
/// Boundary for the Update Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public interface IUpdateSongDirectorBoundary<TOutput>
    : IBoundary<UpdateSongDirectorResponse, TOutput>
{
    /// <summary>
    /// Insufficient permissions.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput InsufficientPermissions(UpdateSongDirectorResponse response);

    /// <summary>
    /// Invalid request.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput InvalidRequest(UpdateSongDirectorResponse response);

    /// <summary>
    /// Requester rank not found.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput RequesterRankNotFound(UpdateSongDirectorResponse response);

    /// <summary>
    /// Song director updated.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput SongDirectorUpdated(UpdateSongDirectorResponse response);

    /// <summary>
    /// Failed to update song director.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput SongDirectorUpdateFailed(UpdateSongDirectorResponse response);
}
