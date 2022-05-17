using FluentResults;

namespace Asaph.Core.UseCases.RemoveSongDirector;

/// <summary>
/// Response for the Remove Song Director use case.
/// </summary>
public class RemoveSongDirectorResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveSongDirectorResponse"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    private RemoveSongDirectorResponse(string message) => Message = message;

    /// <summary>
    /// Message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Response indicating that a song director can't remove themself.
    /// </summary>
    /// <returns><see cref="RemoveSongDirectorResponse"/>.</returns>
    public static RemoveSongDirectorResponse CannotRemoveSelf() => new(
        "You can't remove yourself. If you'd like to be discluded from song director scheduling, " +
        "set yourself as inactive.");

    /// <summary>
    /// Response indicating that the requester doesn't have permissions to remove song directors.
    /// </summary>
    /// <returns><see cref="RemoveSongDirectorResponse"/>.</returns>
    public static RemoveSongDirectorResponse InsufficientPermissions() => new(
        "You don't have permissions to remove song directors.");

    /// <summary>
    /// Response indicating that the removal failed.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="getSongDirectorNameResult">
    /// The result of the attempt to get the song director's name.
    /// </param>
    /// <returns><see cref="RemoveSongDirectorResponse"/>.</returns>
    public static RemoveSongDirectorResponse RemovalFailed(
        string songDirectorId, Result<string> getSongDirectorNameResult)
    {
        return WithDesignation("Failed to remove", songDirectorId, getSongDirectorNameResult);
    }

    /// <summary>
    /// Response indicating that the removal succeeded.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="getSongDirectorNameResult">
    /// The result of the attempt to get the song director's name.
    /// </param>
    /// <returns><see cref="RemoveSongDirectorResponse"/>.</returns>
    public static RemoveSongDirectorResponse SongDirectorRemoved(
        string songDirectorId, Result<string> getSongDirectorNameResult)
    {
        return WithDesignation("Removed", songDirectorId, getSongDirectorNameResult);
    }

    /// <summary>
    /// Gets a response with a message and a song director designation.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="songDirectorId">
    /// Song director id. This is used as the designation if the song director's name couldn't be
    /// retrieved.
    /// </param>
    /// <param name="getSongDirectorNameResult">
    /// The result of the attempt to get the song director's name.
    /// </param>
    /// <returns><see cref="RemoveSongDirectorResponse"/>.</returns>
    private static RemoveSongDirectorResponse WithDesignation(
        string message, string songDirectorId, Result<string> getSongDirectorNameResult)
    {
        string songDirectorDesignation = getSongDirectorNameResult.IsSuccess ?
            $"{getSongDirectorNameResult.Value}." :
            $"song director with id {songDirectorId}.";

        return new($"{message} {songDirectorDesignation}");
    }
}
