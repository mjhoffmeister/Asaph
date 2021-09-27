namespace Asaph.Core.UseCases.GetSongDirectors
{
    public class GetSongDirectorsDefaultBoundary
        : IGetSongDirectorBoundary<GetSongDirectorsResponse>
    {
        public GetSongDirectorsResponse FailedToGetSongDirectors(GetSongDirectorsResponse response)
        {
            throw new System.NotImplementedException();
        }

        public GetSongDirectorsResponse InvalidRequesterEmailAddress(
            GetSongDirectorsResponse response)
        {
            return response;
        }

        public GetSongDirectorsResponse RequestorSongDirectorRankNotFound(
            GetSongDirectorsResponse response)
        {
            return response;
        }

        public GetSongDirectorsResponse Success(GetSongDirectorsResponse response)
        {
            return response;
        }
    }
}
