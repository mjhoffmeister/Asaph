using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.AddSongDirector;

/// <summary>
/// Interactor for the Add Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public class AddSongDirectorInteractor<TOutput>
    : IAsyncUseCaseInteractor<AddSongDirectorRequest, TOutput>
{
    private readonly IAddSongDirectorBoundary<TOutput> _boundary;
    private readonly IAsyncRepository<SongDirector> _songDirectorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddSongDirectorInteractor{TOutput}"/>
    /// class.
    /// </summary>
    /// <param name="songDirectorRepository">Song director repository.</param>
    /// <param name="boundary">Boundary.</param>
    public AddSongDirectorInteractor(
        IAsyncRepository<SongDirector> songDirectorRepository,
        IAddSongDirectorBoundary<TOutput> boundary)
    {
        _songDirectorRepository = songDirectorRepository;
        _boundary = boundary;
    }

    /// <inheritdoc/>
    public async Task<TOutput> HandleAsync(AddSongDirectorRequest request)
    {
        // Reference the requester's id
        string requesterId = request.RequesterId;

        // Get the requester's rank
        Result<Rank?> getRequesterSongDirectorRankResult = await _songDirectorRepository
            .TryFindPropertyByIdAsync<Rank?>(requesterId, nameof(SongDirector.Rank))
            .ConfigureAwait(false);

        // If the requester's rank couldn't be found, return a failure result
        if (getRequesterSongDirectorRankResult.IsFailed)
            return RequesterRankNotFound(requesterId, getRequesterSongDirectorRankResult);

        Rank? requesterRank = getRequesterSongDirectorRankResult.Value;

        // Ensure that the requester is a grandmaster
        if (requesterRank != Rank.Grandmaster)
            return InsufficientPermissions();

        // Return the result of the add attempt
        return await TryAddSongDirector(request).ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to add a song director.
    /// </summary>
    /// <param name="request">The add song director request.</param>
    /// <returns>The result of the request.</returns>
    private async Task<TOutput> TryAddSongDirector(AddSongDirectorRequest request)
    {
        // Validate the new song director
        Result<SongDirector> createSongDirectorResult = SongDirector.TryCreate(
            request.FullName,
            request.EmailAddress,
            request.PhoneNumber,
            request.RankName,
            request.IsActive);

        // Validation failure
        if (createSongDirectorResult.IsFailed)
            return ValidationFailure(createSongDirectorResult);

        SongDirector songDirector = createSongDirectorResult.Value;

        // Add the song director
        Result<SongDirector> addSongDirectorResult = await _songDirectorRepository
            .TryAddAsync(songDirector)
            .ConfigureAwait(false);

        if (addSongDirectorResult.IsFailed)
            return FailedToAddSongDirector(addSongDirectorResult);

        SongDirectorUseCaseModel addedSongDirector = SongDirectorUseCaseModel
            .GrandmasterView(songDirector);

        return SongDirectorAdded(addedSongDirector);
    }

    /// <summary>
    /// Returns output indicating that a song director was added.
    /// </summary>
    /// <param name="addedSongDirector">The added song director.</param>
    /// <returns>The output.</returns>
    private TOutput SongDirectorAdded(SongDirectorUseCaseModel addedSongDirector)
    {
        return _boundary.SongDirectorAdded(
            AddSongDirectorResponse.SongDirectorAdded(addedSongDirector));
    }

    /// <summary>
    /// Returns output indicating that an attempt to add a song director failed.
    /// </summary>
    /// <param name="addSongDirectorResult">The result of the add attempt.</param>
    /// <returns>The output.</returns>
    private TOutput FailedToAddSongDirector(Result<SongDirector> addSongDirectorResult)
    {
        return _boundary.SongDirectorAddFailed(AddSongDirectorResponse.SongDirectorAddFailed(
            $"{addSongDirectorResult.GetErrorMessagesString()}"));
    }

    /// <summary>
    /// Returns output indicating that the requester has insufficient permissions to add a song
    /// director.
    /// </summary>
    /// <returns>The output.</returns>
    private TOutput InsufficientPermissions() => _boundary.InsufficientPermissions(
        AddSongDirectorResponse.InsufficientPermissions());

    /// <summary>
    /// Returns output indicating that the requester's song director rank wasn't found.
    /// </summary>
    /// <param name="requesterId">The id of the requester.</param>
    /// <param name="getRequesterRankResult">
    /// The result of the attempt to get the requester's rank.
    /// </param>
    /// <returns>The output.</returns>
    private TOutput RequesterRankNotFound(
        string requesterId, Result<Rank?> getRequesterRankResult)
    {
        return _boundary.RequesterRankNotFound(
            AddSongDirectorResponse.RequesterRankNotFound(
                requesterId, getRequesterRankResult.GetErrorMessagesString()));
    }

    /// <summary>
    /// Returns output indicating that there was a validation error when trying to create a
    /// song director.
    /// </summary>
    /// <param name="createSongDirectorResult">
    /// The result of the attempt to create a song director.</param>
    /// <returns>The output.</returns>
    private TOutput ValidationFailure(Result<SongDirector> createSongDirectorResult)
    {
        return _boundary.ValidationFailure(
            AddSongDirectorResponse.ValidationFailure(
                createSongDirectorResult.Errors.Select(e => e.Message)));
    }
}