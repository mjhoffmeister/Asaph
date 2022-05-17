namespace Asaph.Core.UseCases.RemoveSongDirector;

/// <summary>
/// Request for removing a song director.
/// </summary>
/// <param name="RequesterId">Requester id.</param>
/// <param name="SongDirectorId">Id of the song director to remove.</param>
public record RemoveSongDirectorRequest(string RequesterId, string SongDirectorId);
