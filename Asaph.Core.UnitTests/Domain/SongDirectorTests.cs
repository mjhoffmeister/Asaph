using Asaph.Core.Domain;
using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using System.Linq;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    /// <summary>
    /// Tests the <see cref="SongDirector"/> class.
    /// </summary>
    public static class SongDirectorTests
    {
        /// <summary>
        /// Tests the
        /// <see cref="SongDirector.TryCreate(string?, string?, string?, string?, bool?)"/> method.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="isActive">Is active.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Apprentice", true, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Journeyer", false, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Master", true, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Grandmaster", true, true)]
        [InlineData(null, "john.doe@mail.com", "123-456-1234", "Apprentice", false, false)]
        [InlineData("John Doe", null, "123-456-1234", "Apprentice", false, false)]
        [InlineData("John Doe", "john.doe@mail.com", null, "Grandmaster", false, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", null, true, true)]
        [InlineData("John Doe", "john.doe@mail.com", null, null, true, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Newbie", true, false)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "", false, false)]
        public static void TryCreate_Multiple_ReturnsExpectedIsSuccess(
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool isActive,
            bool expectedIsSuccess)
        {
            // Act

            Result<SongDirector> songDirectorCreateResult = SongDirector.TryCreate(
                fullName, emailAddress, phoneNumber, rankName, isActive);

            // Assert

            Assert.Equal(expectedIsSuccess, songDirectorCreateResult.IsSuccess);
        }

        /// <summary>
        /// Tests update a user's full name.
        /// </summary>
        /// <param name="isActive">New active indicator.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(null, false)]
        public static void TryUpdateIsActive_Multiple_ReturnsExpectedIsSuccess(
            bool? isActive,
            bool expectedIsSuccess)
        {
            // Arrange

            SongDirector songDirector = SongDirector
                .TryCreate(
                    "Jane Doe",
                    "jane.doe@example.com",
                    "123-456-7890",
                    "Apprentice",
                    true)
                .Value;

            // Act

            Result updateIsActiveResult = songDirector.TryUpdateIsActive(isActive);

            // Assert

            Assert.Equal(expectedIsSuccess, updateIsActiveResult.IsSuccess);
        }

        /// <summary>
        /// Tests that an <see cref="UnchangedPropertyValueError"/> is returned in the result when
        /// an attempt to change a song director's active indicator to the same value is made.
        /// </summary>
        [Fact]
        public static void
            TryUpdateIsActive_UnchangedIsActive_ReturnsUnchangedPropertyValueError()
        {
            // Arrange

            bool isActive = true;

            SongDirector songDirector = SongDirector
                .TryCreate(
                    "Jane Doe",
                    "jane.doe@example.com",
                    "123-456-7890",
                    "Apprentice",
                    isActive)
                .Value;

            // Act

            Result updateIsActiveResult = songDirector.TryUpdateIsActive(isActive);

            // Assert

            Assert.IsType<UnchangedPropertyValueError>(updateIsActiveResult.Errors.Single());
        }

        /// <summary>
        /// Tests update a user's full name.
        /// </summary>
        /// <param name="rank">New rank.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("Apprentice", false)]
        [InlineData("Journeyer", true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, true)]
        public static void TryUpdateRank_Multiple_ReturnsExpectedIsSuccess(
            string? rank, bool expectedIsSuccess)
        {
            // Arrange

            SongDirector songDirector = SongDirector
                .TryCreate(
                    "Jane Doe",
                    "jane.doe@example.com",
                    "123-456-7890",
                    "Apprentice",
                    true)
                .Value;

            // Act

            Result updateRankResult = songDirector.TryUpdateRank(rank);

            // Assert

            Assert.Equal(expectedIsSuccess, updateRankResult.IsSuccess);
        }

        /// <summary>
        /// Tests that an <see cref="UnchangedPropertyValueError"/> is returned in the result when
        /// an attempt to change a song director's rank is made.
        /// </summary>
        [Fact]
        public static void TryUpdateRank_UnchangedRank_ReturnsUnchangedPropertyValueError()
        {
            // Arrange

            string rank = "Apprentice";

            SongDirector songDirector = SongDirector
                .TryCreate(
                    "Jane Doe",
                    "jane.doe@example.com",
                    "123-456-7890",
                    rank,
                    true)
                .Value;

            // Act

            Result updateRankResult = songDirector.TryUpdateRank(rank);

            // Assert

            Assert.IsType<UnchangedPropertyValueError>(updateRankResult.Errors.Single());
        }
    }
}
