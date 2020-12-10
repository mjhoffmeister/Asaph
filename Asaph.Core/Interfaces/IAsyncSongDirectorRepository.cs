using Asaph.Core.Domain.SongDirectorAggregate;
using System;
using System.Threading.Tasks;

namespace Asaph.Core.Interfaces
{
    public interface IAsyncSongDirectorRepository
    {
        /// <summary>
        /// Adds a song director.
        /// </summary>
        /// <param name="songDirector">The song director to add.</param>
        /// <returns>The GUID for the new song director.</returns>
        Task<Guid> AddAsync(SongDirector songDirector);

        /// <summary>
        /// Finds the rank for a song director.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <returns>The song director's rank, if found; null, otherwise.</returns>
        Task<Rank?> FindRankAsync(string emailAddress);
    }
}
