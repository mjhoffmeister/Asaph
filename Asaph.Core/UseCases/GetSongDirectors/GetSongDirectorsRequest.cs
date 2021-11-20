namespace Asaph.Core.UseCases.GetSongDirectors
{
    /// <summary>
    /// Request for getting song directors.
    /// </summary>
    /// <param name="RequesterId">Requester id.</param>
    public record GetSongDirectorsRequest(string RequesterId);
}