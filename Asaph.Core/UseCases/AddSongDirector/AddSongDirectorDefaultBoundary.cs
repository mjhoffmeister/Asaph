namespace Asaph.Core.UseCases.AddSongDirector
{
    /// <summary>
    /// Default boundary for the add song director use case which passes through the response.
    /// </summary>
    public class AddSongDirectorDefaultBoundary : IAddSongDirectorBoundary<AddSongDirectorResponse>
    {
        public AddSongDirectorResponse InsufficientPermissions(AddSongDirectorResponse response) => response;

        public AddSongDirectorResponse SongDirectorAdded(AddSongDirectorResponse response) => response;

        public AddSongDirectorResponse ValidationFailure(AddSongDirectorResponse response) => response;
    }
}
