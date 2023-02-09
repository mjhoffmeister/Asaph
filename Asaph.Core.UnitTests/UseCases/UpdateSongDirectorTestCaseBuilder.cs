using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.UseCases.UpdateSongDirector;
using System.Collections.Generic;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Builds test cases for the Update Song Director use case.
/// </summary>
internal class UpdateSongDirectorTestCaseBuilder
{
    private readonly string? _requesterId;

    private readonly string? _requesterRank;

    private string? _expectedMessage;

    private UpdateSongDirectorRequest? _request;

    private IEnumerable<SongDirector> _existingSongDirectors = new List<SongDirector>();

    private SongDirector? _songDirectorToUpdate;

    private UpdateSongDirectorTestCaseBuilder(string? requesterId, string? requesterRank)
    {
        _requesterId = requesterId;
        _requesterRank = requesterRank;
    }

    /// <summary>
    /// Sets up the requester for the test case.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="requesterRank">Requester rank.</param>
    /// <returns><see cref="UpdateSongDirectorTestCaseBuilder"/>.</returns>
    public static UpdateSongDirectorTestCaseBuilder Requester(
        string? requesterId, string? requesterRank)
    {
        return new(requesterId, requesterRank);
    }

    /// <summary>
    /// Builds the test case.
    /// </summary>
    /// <returns>Test case parameters.</returns>
    public object?[] Build()
    {
        return new object?[]
        {
            _requesterRank,
            _request,
            _songDirectorToUpdate,
            _existingSongDirectors,
            _expectedMessage,
        };
    }

    /// <summary>
    /// Sets existing song directors.
    /// </summary>
    /// <param name="existingSongDirectors">Existing song directors.</param>
    /// <returns><see cref="UpdateSongDirectorTestCaseBuilder"/>.</returns>
    public UpdateSongDirectorTestCaseBuilder ExistingSongDirectors(
        IEnumerable<SongDirector> existingSongDirectors)
    {
        _existingSongDirectors = existingSongDirectors;

        return this;
    }

    /// <summary>
    /// Configures the expected message.
    /// </summary>
    /// <param name="expectedMessage">Expected message.</param>
    /// <returns><see cref="UpdateSongDirectorTestCaseBuilder"/>.</returns>
    public UpdateSongDirectorTestCaseBuilder ExpectedMessage(string expectedMessage)
    {
        _expectedMessage = expectedMessage;

        return this;
    }

    /// <summary>
    /// Configures the request.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="rank">Rank.</param>
    /// <param name="isActive">Active indicator.</param>
    /// <returns><see cref="UpdateSongDirectorTestCaseBuilder"/>.</returns>
    public UpdateSongDirectorTestCaseBuilder Request(
        string? songDirectorId,
        string? fullName,
        string? emailAddress,
        string? phoneNumber,
        string? rank,
        bool? isActive)
    {
        _request = new UpdateSongDirectorRequest(
            _requesterId, songDirectorId, fullName, emailAddress, phoneNumber, rank, isActive);

        return this;
    }

    /// <summary>
    /// Configures the song director to update.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="rank">Rank.</param>
    /// <param name="isActive">Active indicator.</param>
    /// <returns><see cref="UpdateSongDirectorTestCaseBuilder"/>.</returns>
    public UpdateSongDirectorTestCaseBuilder SongDirectorToUpdate(
        string? id,
        string? fullName,
        string? emailAddress,
        string? phoneNumber,
        string? rank,
        bool? isActive)
    {
        SongDirector songDirectorToUpdate = SongDirector
            .TryCreate(fullName, emailAddress, phoneNumber, rank, isActive)
            .Value;

        songDirectorToUpdate.UpdateId(id);

        _songDirectorToUpdate = songDirectorToUpdate;

        return this;
    }
}