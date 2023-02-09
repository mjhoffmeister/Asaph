using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Core.UseCases.UpdateSongDirector;
using FluentResults;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Tests updating song directors.
/// </summary>
public static class UpdateSongDirectorTests
{
    /// <summary>
    /// Tests handling song director updates.
    /// </summary>
    /// <param name="requesterRank">Requester rank.</param>
    /// <param name="request">Request.</param>
    /// <param name="songDirectorToUpdate">Song director to update.</param>
    /// <param name="existingSongDirectors">Existing song directors.</param>
    /// <param name="expectedMessage">Expected message.</param>
    /// <returns>The result of the async task.</returns>
    [Theory]
    [ClassData(typeof(UpdateSongDirectorTestData))]
    public static async Task HandleAsync_Multiple_ReturnsExpectedMessage(
        string requesterRank,
        UpdateSongDirectorRequest request,
        SongDirector songDirectorToUpdate,
        IEnumerable<SongDirector> existingSongDirectors,
        string expectedMessage)
    {
        // Arrange

        IAsyncRepository<SongDirector> songDirectorRepository = GetMockSongDirectorRepository(
            songDirectorToUpdate, request.RequesterId, requesterRank, existingSongDirectors);

        UpdateSongDirectorTestBoundary boundary = new();

        UpdateSongDirectorInteractor<UpdateSongDirectorResponse> addSongDirectorInteractor = new(
            songDirectorRepository, boundary);

        // Act

        UpdateSongDirectorResponse updateSongDirectorResponse = await addSongDirectorInteractor
            .HandleAsync(request)
            .ConfigureAwait(false);

        // Assert

        Assert.Equal(expectedMessage, updateSongDirectorResponse.Message);
    }

    /// <summary>
    /// Returns a mocked song director repository for supporting song director updates.
    /// </summary>
    /// <param name="songDirectorToUpdate">Song director to update..</param>
    /// <param name="requesterId">Requester id.</param>
    /// <param name="requesterRank">Requester rank.</param>
    /// <param name="existingSongDirectors">Existing song directors.</param>
    /// <returns>The mocked song director repository.</returns>
    public static IAsyncRepository<SongDirector> GetMockSongDirectorRepository(
        SongDirector songDirectorToUpdate,
        string? requesterId,
        string requesterRank,
        IEnumerable<SongDirector> existingSongDirectors)
    {
        // Create a mock for the song director repository interface
        Mock<IAsyncRepository<SongDirector>>? mockSongDirectorRepository = new();

        // Configure the TryFindPropertyByIdAsync method to return the song director name
        mockSongDirectorRepository
            .Setup(m => m.TryFindPropertyByIdAsync<string>(
                It.Is<string>(id => id == songDirectorToUpdate.Id),
                It.Is<string>(param => param == nameof(SongDirector.FullName))))
            .Returns(Task.FromResult(Result.Ok(songDirectorToUpdate.FullName)));

        // Configure the TryFindPropertyByIdAsync method to return the requester's rank
        mockSongDirectorRepository
            .Setup(m => m.TryFindPropertyByIdAsync<Rank?>(
                It.Is<string>(id => id == requesterId),
                It.Is<string>(param => param == nameof(SongDirector.Rank))))
            .Returns(Task.FromResult(Result.Ok(Rank.TryGetByName(requesterRank).Value)));

        // Configure the TryGetAll method
        mockSongDirectorRepository
            .Setup(m => m.TryGetAllAsync())
            .Returns(Task.FromResult(Result.Ok(existingSongDirectors)));

        // Configure the TryGetByIdAsync method
        mockSongDirectorRepository
            .Setup(m => m.TryGetByIdAsync(It.Is<string>(id => id == songDirectorToUpdate.Id)))
            .Returns(Task.FromResult(Result.Ok(songDirectorToUpdate)));

        // Configure the TryUpdateAsync method
        mockSongDirectorRepository
            .Setup(m => m.TryUpdateAsync(It.IsAny<SongDirector>()))
            .Returns(Task.FromResult(Result.Ok()));

        return mockSongDirectorRepository.Object;
    }
}
