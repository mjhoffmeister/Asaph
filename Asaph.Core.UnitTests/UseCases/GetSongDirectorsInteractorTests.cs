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
    /// <summary>
    /// Tests the <see cref="GetSongDirectorsInteractor{TOutput}"/> class.
    /// </summary>
    public static class GetSongDirectorsInteractorTests
    {
        /// <summary>
        /// Tests rank returns.
        /// </summary>
        /// <param name="requesterId">Requester id.</param>
        /// <param name="shouldHaveRanks">Indicates whether ranks should be returned.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("1", true)]
        [InlineData("2", false)]
        [InlineData("3", false)]
        [InlineData("4", false)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedRankSettings(
            string requesterId, bool shouldHaveRanks)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterId);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsTestBoundary());

            // Act

            GetSongDirectorsResponse response = await getSongDirectorsInteractor
                .HandleAsync(request)
                .ConfigureAwait(false);

            // Assert

            Assert.All(
                response.SongDirectors!,
                songDirector => Assert.True(songDirector.Rank == null ^ shouldHaveRanks));
        }

        /// <summary>
        /// Tests the IsDeletable setting.
        /// </summary>
        /// <param name="requesterId">Requester id.</param>
        /// <param name="expectedJaneDoeExpectedIsDeletable">
        /// Expected IsDeletable value for Jane Doe.
        /// </param>
        /// <param name="expectedZhangXiaExpectedIsDeletable">
        /// Expected IsDeletable value for Zhang Xia.
        /// </param>
        /// <param name="expectedSatoGotaExpectedIsDeletable">
        /// Expected IsDeletable value for Sato Gota.
        /// </param>
        /// <param name="expectedJohnDoeExpectedIsDeletable">
        /// Expected IsDeletable value for John Doe.
        /// </param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("1", false, true, true, true)]
        [InlineData("2", false, false, false, false)]
        [InlineData("3", false, false, false, false)]
        [InlineData("4", false, false, false, false)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedIsDeletable(
            string requesterId,
            bool expectedJaneDoeExpectedIsDeletable,
            bool expectedZhangXiaExpectedIsDeletable,
            bool expectedSatoGotaExpectedIsDeletable,
            bool expectedJohnDoeExpectedIsDeletable)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterId);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsTestBoundary());

            // Act

            GetSongDirectorsResponse response = await getSongDirectorsInteractor
                .HandleAsync(request)
                .ConfigureAwait(false);

            // Assert

            Assert.Collection(
                response.SongDirectors!,
                janeDoe => Assert.Equal(expectedJaneDoeExpectedIsDeletable, janeDoe.IsDeletable),
                zhangXia => Assert.Equal(expectedZhangXiaExpectedIsDeletable, zhangXia.IsDeletable),
                satoGota => Assert.Equal(expectedSatoGotaExpectedIsDeletable, satoGota.IsDeletable),
                johnDoe => Assert.Equal(expectedJohnDoeExpectedIsDeletable, johnDoe.IsDeletable));
        }

        /// <summary>
        /// Tests the IsEditable setting.
        /// </summary>
        /// <param name="requesterId">Requester id.</param>
        /// <param name="expectedJaneDoeExpectedIsEditable">
        /// Expected IsEditable value for Jane Doe.
        /// </param>
        /// <param name="expectedZhangXiaExpectedIsEditable">
        /// Expected IsEditable value for Zhang Xia.
        /// </param>
        /// <param name="expectedSatoGotaExpectedIsEditable">
        /// Expected IsEditable value for Sato Gota.
        /// </param>
        /// <param name="expectedJohnDoeExpectedIsEditable">
        /// Expected IsEditable value for John Doe.
        /// </param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("1", true, true, true, true)]
        [InlineData("2", false, true, false, false)]
        [InlineData("3", false, false, true, false)]
        [InlineData("4", false, false, false, true)]
        public static async Task Handle_ValidRequester_ReturnsModelsWithExpectedIsEditable(
            string requesterId,
            bool expectedJaneDoeExpectedIsEditable,
            bool expectedZhangXiaExpectedIsEditable,
            bool expectedSatoGotaExpectedIsEditable,
            bool expectedJohnDoeExpectedIsEditable)
        {
            // Arrange

            GetSongDirectorsRequest request = new(requesterId);

            GetSongDirectorsInteractor<GetSongDirectorsResponse> getSongDirectorsInteractor = new(
                GetMockSongDirectorRepository(), new GetSongDirectorsTestBoundary());

            // Act

            GetSongDirectorsResponse response = await getSongDirectorsInteractor
                .HandleAsync(request)
                .ConfigureAwait(false);

            // Assert

            Assert.Collection(
                response.SongDirectors!,
                janeDoe => Assert.Equal(expectedJaneDoeExpectedIsEditable, janeDoe.IsEditable),
                zhangXia => Assert.Equal(expectedZhangXiaExpectedIsEditable, zhangXia.IsEditable),
                satoGota => Assert.Equal(expectedSatoGotaExpectedIsEditable, satoGota.IsEditable),
                johnDoe => Assert.Equal(expectedJohnDoeExpectedIsEditable, johnDoe.IsEditable));
        }

        /// <summary>
        /// Gets a mocked song director repository.
        /// </summary>
        /// <returns>A mock song director repository.</returns>
        private static IAsyncRepository<SongDirector> GetMockSongDirectorRepository()
        {
            Mock<IAsyncRepository<SongDirector>> mock = new();

            SetupTryFindPropertyByIdAsync(mock);

            SetupTryGetAllAsync(mock);

            return mock.Object;
        }

        private static void SetupTryFindPropertyByIdAsync(
            Mock<IAsyncRepository<SongDirector>> mock)
        {
            mock
                .Setup(repo => repo.TryFindPropertyByIdAsync<Rank?>(
                    It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((id, _) => Task.FromResult<Result<Rank?>>(id switch
                {
                    "1" => Result.Ok<Rank?>(Rank.Grandmaster),
                    "2" => Result.Ok<Rank?>(Rank.Journeyer),
                    "3" => Result.Ok<Rank?>(Rank.Apprentice),
                    "4" => Result.Ok<Rank?>(Rank.Apprentice),
                    _ => Result.Fail("Song director not found."),
                }));
        }

        /// <summary>
        /// Sets up the TryGetAllAsync method for a mock song directory repository.
        /// </summary>
        /// <param name="mock">The target mock song director repository.</param>
        private static void SetupTryGetAllAsync(Mock<IAsyncRepository<SongDirector>> mock)
        {
            IEnumerable<SongDirector> songDirectors = new[]
            {
                CreateSongDirector(
                    "1", "Jane Doe", "jane.doe@example.com", "123-456-1234", "Grandmaster", true),
                CreateSongDirector(
                    "2", "Zhang Xia", "zhang.xia@example.com", "234-567-2345", "Journeyer", true),
                CreateSongDirector(
                    "3", "Sato Gota", "sato.gota@example.com", "345-678-3456", "Apprentice", true),
                CreateSongDirector(
                    "4", "John Doe", "john.doe@example.com", "456-478-4567", "Apprentice", false),
            };

            mock
                .Setup(repo => repo.TryGetAllAsync())
                .Returns(Task.FromResult(Result.Ok(songDirectors)));
        }

        /// <summary>
        /// Creates a song director.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="isActive">Active indicator.</param>
        /// <returns><see cref="SongDirector"/>.</returns>
        private static SongDirector CreateSongDirector(
            string id,
            string fullName,
            string emailAddress,
            string phoneNumber,
            string rankName,
            bool isActive)
        {
            SongDirector songDirector = SongDirector
                .TryCreate(fullName, emailAddress, phoneNumber, rankName, isActive)
                .Value;

            songDirector.UpdateId(id);

            return songDirector;
        }
    }
}