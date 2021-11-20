using Asaph.Core.Domain.SongDirectorAggregate;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    /// <summary>
    /// Song director extensions.
    /// </summary>
    internal static class SongDirectorExtensions
    {
        /// <summary>
        /// Converts a song director to a model.
        /// </summary>
        /// <param name="songDirector">The song director to convert.</param>
        /// <param name="requesterId">Requester id.</param>
        /// <param name="requesterRank">Requester rank.</param>
        /// <returns><see cref="SongDirectorUseCaseModel"/>.</returns>
        public static SongDirectorUseCaseModel ConvertToUseCaseModel(
            this SongDirector songDirector, string? requesterId, Rank? requesterRank)
        {
            if (songDirector.Id == requesterId)
                return SongDirectorUseCaseModel.Self(songDirector, requesterRank);

            if (requesterRank == Rank.Grandmaster)
                return SongDirectorUseCaseModel.GrandmasterView(songDirector);

            return SongDirectorUseCaseModel.ReadOnly(songDirector);
        }
    }
}