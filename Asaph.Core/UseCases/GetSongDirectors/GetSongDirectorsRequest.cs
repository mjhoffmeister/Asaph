namespace Asaph.Core.UseCases.GetSongDirectors
{
    public class GetSongDirectorsRequest
    {
        public GetSongDirectorsRequest(string? requesterEmailAddress) =>
            RequesterEmailAddress = requesterEmailAddress;

        public string? RequesterEmailAddress { get; }
    }
}