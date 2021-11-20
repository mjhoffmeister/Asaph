using Asaph.Core.Domain.UserAggregate;
using FluentResults;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    /// <summary>
    /// Tests the User entity.
    /// </summary>
    public static class UserTests
    {
        /// <summary>
        /// Tests creating users.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", true)]
        [InlineData("John Doe", "john.doe@mail.com", null, true)]
        [InlineData("John Doe", "john.doe@mail", "123-456-1234", false)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-123", false)]
        [InlineData("", "john.doe@mail.com", "123-456-1234", false)]
        [InlineData(" ", "john.doe@mail.com", "123-456-1234", false)]
        public static void TryCreate_Multiple_ReturnsExpectedIsSuccess(
            string fullName,
            string emailAddress,
            string? phoneNumber,
            bool expectedIsSuccess)
        {
            // Act

            Result<User> result = User.TryCreate(fullName, emailAddress, phoneNumber);

            // Assert

            Assert.Equal(expectedIsSuccess, result.IsSuccess);
        }
    }
}
