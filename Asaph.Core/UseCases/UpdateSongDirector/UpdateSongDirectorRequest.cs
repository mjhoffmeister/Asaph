namespace Asaph.Core.UseCases.UpdateSongDirector;

/// <summary>
/// Request for updating a song director.
/// </summary>
/// <param name="RequesterId">Requester id.</param>
/// <param name="SongDirectorId">Id of the song director to update.</param>
/// <param name="FullName">Full name.</param>
/// <param name="EmailAddress">Email address.</param>
/// <param name="PhoneNumber">Phone number.</param>
/// <param name="RankName">Rank name.</param>
/// <param name="IsActive">Is active.</param>
public record UpdateSongDirectorRequest(
    string? RequesterId,
    string? SongDirectorId,
    string? FullName,
    string? EmailAddress,
    string? PhoneNumber,
    string? RankName,
    bool? IsActive);
