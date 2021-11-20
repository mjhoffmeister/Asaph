using Asaph.Core.Interfaces;
using Asaph.Infrastructure.Interfaces;
using FluentResults;
using System.Threading.Tasks;

namespace Asaph.Infrastructure.SongDirectorRepository
{
    /// <summary>
    /// Interface for a fragment (i.e. data source) of a song director repository.
    /// </summary>
    public interface ISongDirectorRepositoryFragment
        : IAsyncRepository<SongDirectorDataModel>, IRepositoryFragment
    {
        /// <summary>
        /// Tries to roll back the removal of a song director.
        /// </summary>
        /// <param name="songDirectorDataModel">
        /// The data model for the song director that was removed.
        /// </param>
        /// <returns>The result of the attempt.</returns>
        Task<Result> TryRollBackRemove(SongDirectorDataModel songDirectorDataModel);
    }
}
