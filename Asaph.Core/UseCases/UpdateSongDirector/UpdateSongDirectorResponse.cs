using System;

namespace Asaph.Core.UseCases.UpdateSongDirector;

/// <summary>
/// Update song director response.
/// </summary>
public record UpdateSongDirectorResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSongDirectorResponse"/> record.
    /// </summary>
    /// <param name="message">Message.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "UpdateSongDirectorResponse is a record, not a class.")]
    private UpdateSongDirectorResponse(string message) => Message = message;

    /// <summary>
    /// Message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Creates a response indicating the the requester doesn't have sufficient permissions to
    /// perform the update.
    /// </summary>
    /// <param name="songDirectorFullName">Song director first name.</param>
    /// <returns><see cref="UpdateSongDirectorResponse"/>.</returns>
    public static UpdateSongDirectorResponse InsufficientPermissions(string? songDirectorFullName)
    {
        return new UpdateSongDirectorResponse(
            "You don't have permission to update " +
            $"{(songDirectorFullName != null ? $"{songDirectorFullName}" : "the song director")}.");
    }

    /// <summary>
    /// Creates a response indicating an invalid request.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <returns><see cref="UpdateSongDirectorResponse"/>.</returns>
    public static UpdateSongDirectorResponse InvalidRequest(string message)
    {
        return new UpdateSongDirectorResponse(message);
    }

    /// <summary>
    /// Creates a response indicating that the requester rank wasn't found.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="requesterFullName">Requester full name.</param>
    /// <returns><see cref="UpdateSongDirectorResponse"/>.</returns>
    public static UpdateSongDirectorResponse RequesterRankNotFound(
        string message, string? requesterFullName)
    {
        return new UpdateSongDirectorResponse(
            "Couldn't find requester rank" +
            $"{(requesterFullName != null ? $" for {requesterFullName}" : "")}. {message}");
    }

    /// <summary>
    /// Creates a response indicating that the song director was updated.
    /// </summary>
    /// <param name="songDirectorFullName">The song director's full name.</param>
    /// <returns><see cref="UpdateSongDirectorResponse"/>.</returns>
    public static UpdateSongDirectorResponse SongDirectorUpdated(string songDirectorFullName)
    {
        return new UpdateSongDirectorResponse($"Updated {songDirectorFullName}.");
    }

    /// <summary>
    /// Creates a response indicating that the song director update failed.
    /// </summary>
    /// <param name="songDirectorFullName">Song director full name.</param>
    /// <param name="message">Message.</param>
    /// <returns><see cref="UpdateSongDirectorResponse"/>.</returns>
    public static UpdateSongDirectorResponse SongDirectorUpdateFailed(
        string songDirectorFullName, string message)
    {
        return new UpdateSongDirectorResponse(
            $"Failed to update {songDirectorFullName}. {message}");
    }
}
