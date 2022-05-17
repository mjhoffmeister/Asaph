using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.RemoveSongDirector;

/// <summary>
/// Interactor for the Remove Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public class RemoveSongDirectorInteractor<TOutput>
    : IAsyncUseCaseInteractor<RemoveSongDirectorRequest, TOutput>
{
    private readonly IRemoveSongDirectorBoundary<TOutput> _boundary;
    private readonly ILogger _logger;
    private readonly IAsyncRepository<SongDirector> _songDirectorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveSongDirectorInteractor{TOutput}"/> class.
    /// </summary>
    /// <param name="songDirectorRepository">Song director repository.</param>
    /// <param name="boundary">Boundary.</param>
    /// <param name="logger">Logger.</param>
    public RemoveSongDirectorInteractor(
        IAsyncRepository<SongDirector> songDirectorRepository,
        IRemoveSongDirectorBoundary<TOutput> boundary,
        ILogger logger)
    {
        _boundary = boundary;
        _logger = logger;
        _songDirectorRepository = songDirectorRepository;
    }

    /// <inheritdoc/>
    public async Task<TOutput> HandleAsync(RemoveSongDirectorRequest request)
    {
        // Reference the ids of the requester and song director to be removed
        (string requesterId, string songDirectorId) = request;

        // If a song director is trying to remove themself, return a response that it isn't allowed
        if (requesterId == songDirectorId)
            return _boundary.CannotRemoveSelf(RemoveSongDirectorResponse.CannotRemoveSelf());

        // Try to find the requester's song director rank
        Result<Rank?> getRequesterSongDirectorRankResult = await _songDirectorRepository
            .TryFindPropertyByIdAsync<Rank?>(requesterId, nameof(SongDirector.Rank))
            .ConfigureAwait(false);

        // Return an insufficient permissions response if the requester isn't a grandmaster
        if (getRequesterSongDirectorRankResult.Value != Rank.Grandmaster)
        {
            return _boundary.InsufficientPermissions(
                RemoveSongDirectorResponse.InsufficientPermissions());
        }

        // Get the name of the song director to be removed
        Result<string> getSongDirectorNameResult = await _songDirectorRepository
            .TryFindPropertyByIdAsync<string>(songDirectorId, nameof(SongDirector.FullName))
            .ConfigureAwait(false);

        // Try to remove the song director
        Result removeSongDirectorResult = await _songDirectorRepository
            .TryRemoveByIdAsync(songDirectorId)
            .ConfigureAwait(false);

        // If the removal failed, return a failure result
        if (removeSongDirectorResult.IsFailed)
        {
            _logger.LogError(
                "Failed to remove song director. {ErrorMessage}",
                removeSongDirectorResult.GetErrorMessagesString());

            return _boundary.RemovalFailed(RemoveSongDirectorResponse
                .RemovalFailed(songDirectorId, getSongDirectorNameResult));
        }

        // If the removal was a success, return a success result
        return _boundary.SongDirectorRemoved(RemoveSongDirectorResponse
            .SongDirectorRemoved(songDirectorId, getSongDirectorNameResult));
    }
}
