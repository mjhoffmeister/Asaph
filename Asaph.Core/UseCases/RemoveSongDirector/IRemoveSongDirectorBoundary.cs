namespace Asaph.Core.UseCases.RemoveSongDirector;

/// <summary>
/// Boundary interface for the Remove Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public interface IRemoveSongDirectorBoundary<TOutput>
    : IBoundary<RemoveSongDirectorResponse, TOutput>
{
    /// <summary>
    /// Cannot remove self.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput CannotRemoveSelf(RemoveSongDirectorResponse response);

    /// <summary>
    /// Insufficient permissions.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput InsufficientPermissions(RemoveSongDirectorResponse response);

    /// <summary>
    /// Removal failed.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput RemovalFailed(RemoveSongDirectorResponse response);

    /// <summary>
    /// Song director removed.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Output.</returns>
    TOutput SongDirectorRemoved(RemoveSongDirectorResponse response);
}
