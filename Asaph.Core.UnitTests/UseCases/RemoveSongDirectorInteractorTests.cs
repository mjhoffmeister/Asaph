using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Core.UseCases.RemoveSongDirector;
using FluentResults;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Tests the <see cref="RemoveSongDirectorInteractor{TOutput}"/> class.
/// </summary>
public static class RemoveSongDirectorInteractorTests
{
    /// <summary>
    /// Tests whether the expected message is returned when a song director is requested to be
    /// removed.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="requesterRank">Requested rank.</param>
    /// <param name="songDirectorId">Id of the song director to remove.</param>
    /// <param name="songDirectorName">Name of the song director to remove.</param>
    /// <param name="expectedMessage">The expected message.</param>
    /// <returns>The result of the async operation.</returns>
    [Theory]
    [ClassData(typeof(RemoveSongDirectorTestData))]
    public static async Task HandleAsync_MultipleRequests_ReturnsExpectedMessage(
        string requesterId,
        string requesterRank,
        string songDirectorId,
        string songDirectorName,
        string expectedMessage)
    {
        // Arrange

        IAsyncRepository<SongDirector> songDirectorRepository = GetMockSongDirectorRepository(
            requesterId, requesterRank, songDirectorId, songDirectorName);

        RemoveSongDirectorTestBoundary boundary = new();

        RemoveSongDirectorRequest removeSongDirectorRequest = new(requesterId, songDirectorId);

        RemoveSongDirectorInteractor<RemoveSongDirectorResponse> removeSongDirectorInteractor = new(
            songDirectorRepository, boundary, NullLogger.Instance);

        // Act

        RemoveSongDirectorResponse response = await removeSongDirectorInteractor
            .HandleAsync(removeSongDirectorRequest)
            .ConfigureAwait(false);

        // Assert

        Assert.Equal(expectedMessage, response.Message);
    }

    /// <summary>
    /// Returns a mocked song director repository for supporting song director removals.
    /// </summary>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="requesterRank">Requester rank.</param>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="songDirectorName">Song director name.</param>
    /// <returns>The mocked song director repository.</returns>
    private static IAsyncRepository<SongDirector> GetMockSongDirectorRepository(
        string requesterId, string requesterRank, string songDirectorId, string songDirectorName)
    {
        // Create a mock for the song director repository interface
        Mock<IAsyncRepository<SongDirector>> mockSongDirectorRepository = new();

        // Configure the call for getting the requester's rank
        mockSongDirectorRepository
            .Setup(m => m.TryFindPropertyByIdAsync<Rank?>(
                It.Is<string>(id => id == requesterId),
                It.Is<string>(param => param == nameof(SongDirector.Rank))))
            .Returns(Task.FromResult(Result.Ok(Rank.TryGetByName(requesterRank).Value)));

        // Configure the call for getting the name of the song director to delete
        mockSongDirectorRepository
           .Setup(m => m.TryFindPropertyByIdAsync<string>(
               It.Is<string>(id => id == songDirectorId),
               It.Is<string>(param => param == nameof(SongDirector.FullName))))
           .Returns(Task.FromResult(Result.Ok(songDirectorName)));

        // Configure the RemoveAsync method
        mockSongDirectorRepository
            .Setup(m => m.TryRemoveByIdAsync(It.Is<string>(id => id == songDirectorId)))
            .Returns(Task.FromResult(Result.Ok()));

        return mockSongDirectorRepository.Object;
    }
}
