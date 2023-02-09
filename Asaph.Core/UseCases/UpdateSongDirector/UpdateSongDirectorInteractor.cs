using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.UpdateSongDirector;

/// <summary>
/// Interactor for the Update Song Director use case.
/// </summary>
/// <typeparam name="TOutput">Output type.</typeparam>
public class UpdateSongDirectorInteractor<TOutput>
    : IAsyncUseCaseInteractor<UpdateSongDirectorRequest, TOutput>
{
    // Use case boundary
    private readonly IUpdateSongDirectorBoundary<TOutput> _boundary;

    // Song director repository
    private readonly IAsyncRepository<SongDirector> _songDirectorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSongDirectorInteractor{TOutput}"/> class.
    /// </summary>
    /// <param name="songDirectorRepository">Song director repository.</param>
    /// <param name="boundary">Boundary.</param>
    public UpdateSongDirectorInteractor(
        IAsyncRepository<SongDirector> songDirectorRepository,
        IUpdateSongDirectorBoundary<TOutput> boundary)
    {
        _boundary = boundary;
        _songDirectorRepository = songDirectorRepository;
    }

    /// <inheritdoc/>
    public async Task<TOutput> HandleAsync(UpdateSongDirectorRequest request)
    {
        // Validate requester id
        Result<string> requesterIdValidation = ValidateId("Requester", request.RequesterId);

        // If the validation failed, output invalid request
        if (requesterIdValidation.IsFailed)
            return InvalidRequest(requesterIdValidation.GetErrorMessagesString());

        // Validate song director id
        Result<string> songDirectorIdValidation = ValidateId(
            "Song director", request.SongDirectorId);

        // If the song director id validation failed, output invalid request
        if (songDirectorIdValidation.IsFailed)
            return InvalidRequest(songDirectorIdValidation.GetErrorMessagesString());

        // Reference the validated ids
        string requesterId = requesterIdValidation.Value;
        string songDirectorId = songDirectorIdValidation.Value;

        // Get the requester's rank
        Result<Rank?> getRequesterRankResult = await _songDirectorRepository
            .TryFindPropertyByIdAsync<Rank?>(requesterId, nameof(SongDirector.Rank))
            .ConfigureAwait(false);

        // If the requester's rank couldn't be retrieved, return requester rank not found
        if (getRequesterRankResult.IsFailed)
        {
            return await RequesterRankNotFound(
                requesterId, getRequesterRankResult.GetErrorMessagesString())
                .ConfigureAwait(false);
        }

        // Reference the requester's rank
        Rank? requesterRank = getRequesterRankResult.Value;

        // Try to update if a song director is updating their own information
        if (requesterId == songDirectorId)
            return await TryUpdateSelf(requesterId, requesterRank, request).ConfigureAwait(false);

        // If the requester isn't a grandmaster, return insufficient permissions
        if (getRequesterRankResult.Value != Rank.Grandmaster)
            return await InsufficientPermissions(songDirectorId).ConfigureAwait(false);

        // Try to update the song director and return the output
        return await TryUpdateSongDirector(request).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates an id.
    /// </summary>
    /// <param name="label">Label for the id. Used in failure messages.</param>
    /// <param name="id">Id.</param>
    /// <returns>The validation result.</returns>
    private static Result<string> ValidateId(string label, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Fail($"{label} id is required.");

        return Result.Ok(id!);
    }

    /// <summary>
    /// Outputs insufficient permissions.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <returns>Output.</returns>
    private async Task<TOutput> InsufficientPermissions(string songDirectorId)
    {
        Result<string> getSongDirectorFullName = await _songDirectorRepository
            .TryFindPropertyByIdAsync<string>(songDirectorId, nameof(SongDirector.FullName))
            .ConfigureAwait(false);

        return _boundary.InsufficientPermissions(
            UpdateSongDirectorResponse.InsufficientPermissions(
                getSongDirectorFullName.ValueOrDefault));
    }

    /// <summary>
    /// Outputs song director updated.
    /// </summary>
    /// <param name="songDirectorFullName">Song director full name.</param>
    /// <returns>Output.</returns>
    private TOutput SongDirectorUpdated(string songDirectorFullName)
    {
        return _boundary.SongDirectorUpdated(
            UpdateSongDirectorResponse.SongDirectorUpdated(songDirectorFullName));
    }

    /// <summary>
    /// Outputs failed to update song director.
    /// </summary>
    /// <param name="songDirectorFullName">Song director full name.</param>
    /// <param name="failureResult">Failure result.</param>
    /// <returns>Output.</returns>
    private TOutput SongDirectorUpdateFailed(string songDirectorFullName, Result failureResult)
    {
        return _boundary.SongDirectorUpdateFailed(
            UpdateSongDirectorResponse.SongDirectorUpdateFailed(
                songDirectorFullName, failureResult.GetErrorMessagesString()));
    }

    /// <summary>
    /// Outputs invalid request.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <returns>Output.</returns>
    private TOutput InvalidRequest(string message)
    {
        return _boundary.InvalidRequest(UpdateSongDirectorResponse.InvalidRequest(message));
    }

    /// <summary>
    /// Outputs requester rank not found.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="message">Message.</param>
    /// <returns>Output.</returns>
    private async Task<TOutput> RequesterRankNotFound(string requesterId, string message)
    {
        Result<string> getRequesterFullName = await _songDirectorRepository
            .TryFindPropertyByIdAsync<string>(requesterId, nameof(SongDirector.FullName))
            .ConfigureAwait(false);

        // Return the output
        return _boundary.RequesterRankNotFound(
            UpdateSongDirectorResponse.RequesterRankNotFound(
                message, getRequesterFullName.ValueOrDefault));
    }

    /// <summary>
    /// Tries to make updates a song director is requesting for themself.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="requesterRank">Requester rank.</param>
    /// <param name="request">Request.</param>
    /// <returns>Output.</returns>
    private async Task<TOutput> TryUpdateSelf(
        string requesterId, Rank? requesterRank, UpdateSongDirectorRequest request)
    {
        Result<Rank?> parseNewRank = Rank.TryGetByName(request.RankName);

        if (parseNewRank.IsFailed)
            return InvalidRequest(parseNewRank.GetErrorMessagesString());

        Rank? newRank = parseNewRank.Value;

        if (requesterRank != Rank.Grandmaster || newRank == Rank.Grandmaster)
            return await TryUpdateSongDirector(request).ConfigureAwait(false);

        Result updateGrandmasterRankValidation = await ValidateGrandmasterRankUpdate(
            requesterId, newRank).ConfigureAwait(false);

        if (updateGrandmasterRankValidation.IsFailed)
            return InvalidRequest(updateGrandmasterRankValidation.GetErrorMessagesString());

        return await TryUpdateSongDirector(request).ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to update a song director.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <returns>Output.</returns>
    private async Task<TOutput> TryUpdateSongDirector(UpdateSongDirectorRequest request)
    {
        // Try to create a song director from the request
        Result<SongDirector> createSongDirectorResult = SongDirector
            .TryCreate(
                request.FullName,
                request.EmailAddress,
                request.PhoneNumber,
                request.RankName,
                request.IsActive);

        // If the song director creation failed, return invalid request
        if (createSongDirectorResult.IsFailed)
            return InvalidRequest(createSongDirectorResult.GetErrorMessagesString());

        // Reference the song director and set their id
        SongDirector songDirector = createSongDirectorResult.Value;
        songDirector.UpdateId(request.SongDirectorId);

        // Try to update the song director
        Result updateSongDirectorResult = await _songDirectorRepository
            .TryUpdateAsync(songDirector)
            .ConfigureAwait(false);

        // If the update failed, return song director update failed
        if (updateSongDirectorResult.IsFailed)
            return SongDirectorUpdateFailed(songDirector.FullName, updateSongDirectorResult);

        // Output song director updated
        return SongDirectorUpdated(songDirector.FullName);
    }

    /// <summary>
    /// Validates a grandmaster rank change.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="newRank">New rank.</param>
    /// <returns>The validation result.</returns>
    private async Task<Result> ValidateGrandmasterRankUpdate(string requesterId, Rank? newRank)
    {
        // If there's no rank change, return an ok result
        if (newRank == Rank.Grandmaster)
            return Result.Ok();

        // Get song directors for evaluating if there are additional grandmasters
        Result<IEnumerable<SongDirector>> getSongDirectors = await _songDirectorRepository
            .TryGetAllAsync()
            .ConfigureAwait(false);

        // Return a failure result if song directors couldn't be retrieved
        if (getSongDirectors.IsFailed)
        {
            return Result.Fail(
                "Could not validate that demoting a grandmaster would not result in there" +
                $"being no remaining grandmasters. " +
                $"{getSongDirectors.GetErrorMessagesString()}");
        }

        // Determine if there is an additional grandmaster
        bool isAdditionalGrandmaster = getSongDirectors.Value
            .Where(songDirector => songDirector.Id != requesterId)
            .Any(songDirector => songDirector.Rank == Rank.Grandmaster);

        // If there isn't, return a failure result
        if (!isAdditionalGrandmaster)
        {
            return Result.Fail(
                "You must promote another song director to grandmaster before demoting " +
                "yourself.");
        }

        // Otherwise, return an ok result
        return Result.Ok();
    }
}
