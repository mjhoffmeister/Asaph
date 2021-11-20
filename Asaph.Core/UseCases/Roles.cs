using Asaph.Core.Domain.SongDirectorAggregate;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.Core.UseCases
{
    /// <summary>
    /// Roles.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Song director rank.
        /// </summary>
        public static readonly string SongDirectorRank = "SongDirector.Rank";

        /// <summary>
        /// Song director grandmaster role.
        /// </summary>
        public static readonly string GrandmasterSongDirector =
            $"{SongDirectorRank}:{Rank.Grandmaster.Name}";

        /// <summary>
        /// Gets song director rank names.
        /// </summary>
        /// <returns>Song director ranks names.</returns>
        public static IEnumerable<string> GetSongDirectorRankNames() =>
            Rank.Enumerate().Select(rank => rank.Name);
    }
}
