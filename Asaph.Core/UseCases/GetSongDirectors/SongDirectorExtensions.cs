using Asaph.Core.Domain.SongDirectorAggregate;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    internal static class SongDirectorExtensions
    {
        public static SongDirectorModel ConvertToModel(
            this SongDirector songDirector, string? requesterEmailAddress, Rank? requesterRank)
        {
            if (songDirector.EmailAddress == requesterEmailAddress)
                return SongDirectorModel.Self(songDirector, requesterRank);

            if (requesterRank == Rank.Grandmaster)
                return SongDirectorModel.GrandmasterView(songDirector);

            return SongDirectorModel.ReadOnly(songDirector);
        }
    }
}
