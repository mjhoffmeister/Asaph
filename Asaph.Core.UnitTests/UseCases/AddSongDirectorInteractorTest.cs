using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Core.UseCases.AddSongDirector;
using FluentResults;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Core.UnitTests.UseCases
{
    /// <summary>
    /// Tests the <see cref="AddSongDirectorInteractor{TOutput}"/> class.
    /// </summary>
    public static class AddSongDirectorInteractorTest
    {
        /// <summary>
        /// Tests whether the expected message is returned when a song director is added.
        /// </summary>
        /// <param name="requesterRank">Requester rank.</param>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="isActive">Is active.</param>
        /// <param name="expectedMessage">Expected message.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [ClassData(typeof(AddSongDirectorInteractorTestData))]
        public static async Task HandleAsync_ValidSongDirector_ReturnsExpectedMessage(
            string requesterRank,
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool? isActive,
            string expectedMessage)
        {
            // Arrange

            string requesterId = Guid.NewGuid().ToString();

            IAsyncRepository<SongDirector> songDirectorRepository = GetMockSongDirectorRepository(
                requesterRank);

            AddSongDirectorDefaultBoundary boundary = new();

            AddSongDirectorInteractor<AddSongDirectorResponse> addSongDirectorInteractor =
                new(songDirectorRepository, boundary);

            AddSongDirectorRequest addSongDirectorRequest = new(
                requesterId, fullName, emailAddress, phoneNumber, rankName, isActive);

            // Act

            AddSongDirectorResponse addSongDirectorResponse = await addSongDirectorInteractor
                .HandleAsync(addSongDirectorRequest)
                .ConfigureAwait(false);

            // Assert

            Assert.Equal(expectedMessage, addSongDirectorResponse.Messages.First());
        }

        /// <summary>
        /// Returns a mocked song director repository for supporting song director adds.
        /// </summary>
        /// <returns>The mocked song director repository.</returns>
        private static IAsyncRepository<SongDirector> GetMockSongDirectorRepository(string rankName)
        {
            // Create a mock for the song director repository interface
            Mock<IAsyncRepository<SongDirector>>? mockSongDirectorRepository = new();

            mockSongDirectorRepository
                .Setup(m => m.TryFindPropertyByIdAsync<Rank?>(
                    It.IsAny<string>(), It.Is<string>(param => param == nameof(SongDirector.Rank))))
                .Returns(Task.FromResult(Result.Ok(Rank.TryGetByName(rankName).Value)));

            // Configure the TryAddAsync method
            mockSongDirectorRepository
                .Setup(m => m.TryAddAsync(It.IsAny<SongDirector>()))
                .Returns(Task.FromResult(Result.Ok(Guid.NewGuid().ToString())));

            return mockSongDirectorRepository.Object;
        }
    }
}
