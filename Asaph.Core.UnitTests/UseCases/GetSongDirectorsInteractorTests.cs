using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Core.UseCases.GetSongDirectors;
using FluentResults;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Core.UnitTests.UseCases
{
    public static class GetSongDirectorsInteractorTests
    {
        [Theory]
        [InlineData("jane.doe@example.com", true)]
        [InlineData("zhang.xia@example.com", false)]
        [InlineData("sato.gota@example.com", false)]
        [InlineData("john.doe@example.com", false)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedRankSettings(
            string requesterEmailAddress, bool shouldHaveRanks)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterEmailAddress);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsDefaultBoundary());

            // Act

            GetSongDirectorsResponse response =
                await getSongDirectorsInteractor.HandleAsync(request);

            // Assert

            Assert.All(
                response.SongDirectors,
                songDirector => Assert.True(songDirector.Rank == null ^ shouldHaveRanks));
        }

        [Theory]
        [InlineData("jane.doe@example.com", false, true, true, true)]
        [InlineData("zhang.xia@example.com", false, false, false, false)]
        [InlineData("sato.gota@example.com", false, false, false, false)]
        [InlineData("john.doe@example.com", false, false, false, false)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedIsDeletable(
            string requesterEmailAddress,
            bool expectedJaneDoeExpectedIsDeletable,
            bool expectedZhangXiaExpectedIsDeletable,
            bool expectedSatoGotaExpectedIsDeletable,
            bool expectedJohnDoeExpectedIsDeletable)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterEmailAddress);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsDefaultBoundary());

            // Act

            GetSongDirectorsResponse response =
                await getSongDirectorsInteractor.HandleAsync(request);

            // Assert

            Assert.Collection(response.SongDirectors,
                janeDoe => Assert.Equal(expectedJaneDoeExpectedIsDeletable, janeDoe.IsDeletable),
                zhangXia => Assert.Equal(expectedZhangXiaExpectedIsDeletable, zhangXia.IsDeletable),
                satoGota => Assert.Equal(expectedSatoGotaExpectedIsDeletable, satoGota.IsDeletable),
                johnDoe => Assert.Equal(expectedJohnDoeExpectedIsDeletable, johnDoe.IsDeletable));
        }

        [Theory]
        [InlineData("jane.doe@example.com", true, true, true, true)]
        [InlineData("zhang.xia@example.com", false, true, false, false)]
        [InlineData("sato.gota@example.com", false, false, true, false)]
        [InlineData("john.doe@example.com", false, false, false, true)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedIsEditable(
            string requesterEmailAddress,
            bool expectedJaneDoeExpectedIsEditable,
            bool expectedZhangXiaExpectedIsEditable,
            bool expectedSatoGotaExpectedIsEditable,
            bool expectedJohnDoeExpectedIsEditable)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterEmailAddress);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsDefaultBoundary());

            // Act

            GetSongDirectorsResponse response =
                await getSongDirectorsInteractor.HandleAsync(request);

            // Assert

            Assert.Collection(response.SongDirectors,
                janeDoe => Assert.Equal(expectedJaneDoeExpectedIsEditable, janeDoe.IsEditable),
                zhangXia => Assert.Equal(expectedZhangXiaExpectedIsEditable, zhangXia.IsEditable),
                satoGota => Assert.Equal(expectedSatoGotaExpectedIsEditable, satoGota.IsEditable),
                johnDoe => Assert.Equal(expectedJohnDoeExpectedIsEditable, johnDoe.IsEditable));
        }

        /// <summary>
        /// Gets a mocked song director repository.
        /// </summary>
        /// <returns>A mock song director repository.</returns>
        private static IAsyncSongDirectorRepository GetMockSongDirectorRepository()
        {
            Mock<IAsyncSongDirectorRepository> mock = new();

            SetupTryFindRankAsync(mock);

            SetupTryGetAllAsync(mock);

            return mock.Object;
        }

        /// <summary>
        /// Sets up the TryFindRankAsync method for a mock song director repository.
        /// </summary>
        /// <param name="mock">The target mock song director repository.</param>
        private static void SetupTryFindRankAsync(Mock<IAsyncSongDirectorRepository> mock)
        {
            mock
                .Setup(repo => repo.TryFindRankAsync(It.IsAny<string>()))
                .Returns<string>(emailAddress => Task.FromResult<Result<Rank?>>(emailAddress switch
                {
                    "jane.doe@example.com" => Result.Ok((Rank?)Rank.Grandmaster),
                    "zhang.xia@example.com" => Result.Ok((Rank?)Rank.Journeyer),
                    "sato.gota@example.com" => Result.Ok((Rank?)Rank.Apprentice),
                    "john.doe@example.com" => Result.Ok((Rank?)Rank.Apprentice),
                    _ => Result.Fail("Song director not found.")
                }));
        }

        /// <summary>
        /// Sets up the TryGetAllAsync method for a mock song directory repository.
        /// </summary>
        /// <param name="mock">The target mock song director repository.</param>
        private static void SetupTryGetAllAsync(Mock<IAsyncSongDirectorRepository> mock)
        {
            IEnumerable<SongDirector> songDirectors = new[]
            {
                SongDirector
                    .TryCreate(
                        "Jane Doe", "jane.doe@example.com", "123-456-1234", "Grandmaster", true)
                    .Value,
                SongDirector
                    .TryCreate(
                        "Zhang Xia", "zhang.xia@example.com", "234-567-2345", "Journeyer", true)
                    .Value,
                SongDirector
                    .TryCreate(
                        "Sato Gota", "sato.gota@example.com", "345-678-3456", "Apprentice", true)
                    .Value,
                SongDirector
                    .TryCreate(
                        "John Doe", "john.doe@example.com", "456-478-4567", "Apprentice", false)
                    .Value
            };

            mock
                .Setup(repo => repo.TryGetAllAsync())
                .Returns(Task.FromResult(Result.Ok(songDirectors)));
        }
    }
}
