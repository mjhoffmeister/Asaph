using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    /// <summary>
    /// Interactor for the Get Song Directors use case.
    /// </summary>
    /// <typeparam name="TOutput">Output type.</typeparam>
    public class GetSongDirectorsInteractor<TOutput> :
        IAsyncUseCaseInteractor<GetSongDirectorsRequest, TOutput>
    {
        private readonly IGetSongDirectorsBoundary<TOutput> _boundary;
        private readonly IAsyncRepository<SongDirector> _songDirectorRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSongDirectorsInteractor{TOutput}"/>
        /// class.
        /// </summary>
        /// <param name="songDirectorRepository">Song director repository.</param>
        /// <param name="boundary">Boundary.</param>
        public GetSongDirectorsInteractor(
            IAsyncRepository<SongDirector> songDirectorRepository,
            IGetSongDirectorsBoundary<TOutput> boundary)
        {
            _boundary = boundary;
            _songDirectorRepository = songDirectorRepository;
        }

        /// <inheritdoc/>
        public async Task<TOutput> HandleAsync(GetSongDirectorsRequest request)
        {
            // Reference the requester's id
            string requesterId = request.RequesterId;

            // Try to find the requester's song director rank
            Result<Rank?> findRankResult = await _songDirectorRepository
                .TryFindPropertyByIdAsync<Rank?>(requesterId, nameof(SongDirector.Rank))
                .ConfigureAwait(false);

            // Get song directors from the repository
            Result<IEnumerable<SongDirector>> getSongDirectorsResult = await _songDirectorRepository
                .TryGetAllAsync()
                .ConfigureAwait(false);

            // If the request failed, return a failure result
            if (getSongDirectorsResult.IsFailed)
                return FailedToGetSongDirectors(getSongDirectorsResult.GetErrorMessagesString());

            // Convert song director entities to models
            IEnumerable<SongDirectorUseCaseModel> songDirectorModels = getSongDirectorsResult.Value!
                .Select(songDirector =>
                    songDirector.ConvertToUseCaseModel(requesterId, findRankResult.Value));

            System.Console.WriteLine(
                $"Song directors retrieved. Requester rank for id {request.RequesterId} is " +
                $"{findRankResult.ValueOrDefault?.Name ?? "not set"}");

            // Return a success response
            return Success(songDirectorModels);
        }

        private TOutput FailedToGetSongDirectors(string errorMessage)
        {
            return _boundary.FailedToGetSongDirectors(
                GetSongDirectorsResponse.FailedToGetSongDirectors(errorMessage));
        }

        private TOutput Success(IEnumerable<SongDirectorUseCaseModel> songDirectorModels) =>
            _boundary.Success(GetSongDirectorsResponse.Success(songDirectorModels));
    }
}
