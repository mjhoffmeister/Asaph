using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Asaph.Core.Interfaces
{
    public interface IAsyncSongDirectorRepository
    {
        /// <summary>
        /// Tries to add a song director.
        /// </summary>
        /// <param name="songDirector">The song director to add.</param>
        /// <returns>The GUID for the new song director.</returns>
        Task<Result> TryAddAsync(SongDirector songDirector);

        /// <summary>
        /// Tries to find the rank for a song director.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <returns>The song director's rank, if found; null, otherwise.</returns>
        Task<Result<Rank?>> TryFindRankAsync(string emailAddress);

        /// <summary>
        /// Tries to get all song directors.
        /// </summary>
        /// <returns>
        /// A result with a collection of song directors if they could be retrieved. Otherwise,
        /// details about any errors that were encountered.
        /// </returns>
        Task<Result<IEnumerable<SongDirector>>> TryGetAllAsync();

        /// <summary>
        /// Tries to get all ranks.
        /// </summary>
        /// <returns>
        /// A result with a collection of ranks if they could be retrieved. Otherwise, details about
        /// any errors that were encountered.
        /// </returns>
        Task<Result<IEnumerable<Rank>>> TryGetRanks();
    }
}
