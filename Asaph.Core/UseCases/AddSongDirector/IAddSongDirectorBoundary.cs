namespace Asaph.Core.UseCases.AddSongDirector
{
    public interface IAddSongDirectorBoundary<TOutput> : IBoundary<AddSongDirectorResponse, TOutput>
    {
        TOutput InsufficientPermissions(AddSongDirectorResponse response);

        TOutput RequesterRankNotFound(AddSongDirectorResponse response);

        TOutput SongDirectorAdded(AddSongDirectorResponse response);

        TOutput SongDirectorAddFailed(AddSongDirectorResponse response);

        TOutput ValidationFailure(AddSongDirectorResponse response);
    }
}
