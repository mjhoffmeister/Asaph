using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    public static class SongDirectorTest
    {
        [Theory]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Apprentice", true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Journeyer", true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Master", true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Grandmaster", true)]
        [InlineData(null, "john.doe@mail.com", "123-456-1234", "Apprentice", false)]
        [InlineData("John Doe", null, "123-456-1234", "Apprentice", false)]
        [InlineData("John Doe", "john.doe@mail.com", null, "Grandmaster", true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", null, true)]
        [InlineData("John Doe", "john.doe@mail.com", null, null, true)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "Newbie", false)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", "", false)]
        public static void TryCreate_Multiple_ReturnsExpectedIsSuccess(
            string? fullName, string? emailAddress, string? phoneNumber, string? rankName, bool expectedIsSuccess)
        {
            // Act

            Result<SongDirector> songDirectorCreateResult = SongDirector.TryCreate(
                fullName, emailAddress, phoneNumber, rankName);

            // Assert

            Assert.Equal(expectedIsSuccess, songDirectorCreateResult.IsSuccess);
        }
    }
}
