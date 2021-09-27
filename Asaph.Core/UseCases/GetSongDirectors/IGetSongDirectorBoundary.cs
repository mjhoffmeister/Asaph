namespace Asaph.Core.UseCases.GetSongDirectors
{
    public interface IGetSongDirectorBoundary<TOutput>
    {
        TOutput FailedToGetSongDirectors(GetSongDirectorsResponse response);

        TOutput InvalidRequesterEmailAddress(GetSongDirectorsResponse response);

        TOutput RequestorSongDirectorRankNotFound(GetSongDirectorsResponse response);

        TOutput Success(GetSongDirectorsResponse response);
    }
}