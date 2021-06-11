using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    public static class SongDirectorTest
    {
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
            string? fullName, string? emailAddress, string? phoneNumber, string? rankName, bool isActive, bool expectedIsSuccess)
        {
            // Act

            Result<SongDirector> songDirectorCreateResult = SongDirector.TryCreate(
                fullName, emailAddress, phoneNumber, rankName, isActive);

            // Assert

            Assert.Equal(expectedIsSuccess, songDirectorCreateResult.IsSuccess);
        }
    }
}
