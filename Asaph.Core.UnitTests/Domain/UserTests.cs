using Asaph.Core.Domain;
using Asaph.Core.Domain.UserAggregate;
using FluentResults;
using System.Linq;
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

        /// <summary>
        /// Tests updating a user's email address.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("vera.ilyinichna@example.com", false)]
        [InlineData("vera.ilyinichna@example2.com", true)]
        [InlineData("vera.ilyinichnaexample2.com", false)]
        [InlineData("vera.ilyinichna@example2com", false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public static void TryUpdateEmailAddress_Multiple_ReturnsExpectedIsSuccess(
            string? emailAddress,
            bool expectedIsSuccess)
        {
            // Arrange

            User user = User
                .TryCreate(
                    "Vera Ilyinichna",
                    "vera.ilyinichna@example.com",
                    "123-456-7890")
                .Value;

            // Act

            Result updateEmailAddressResult = user.TryUpdateEmailAddress(emailAddress);

            // Assert

            Assert.Equal(expectedIsSuccess, updateEmailAddressResult.IsSuccess);
        }

        /// <summary>
        /// Tests that an <see cref="UnchangedPropertyValueError"/> is returned in the result when
        /// an attempt to change a user's email address with the same value is made.
        /// </summary>
        [Fact]
        public static void
            TryUpdateEmailAddress_UnchangedEmailAddress_ReturnsUnchangedPropertyValueError()
        {
            // Arrange

            string emailAddress = "vera.ilyinichna@example.com";

            User user = User
                .TryCreate(
                    "Vera Ilyinichna",
                    emailAddress,
                    "123-456-7890")
                .Value;

            // Act

            Result updateEmailAddressResult = user.TryUpdateEmailAddress(emailAddress);

            // Assert

            Assert.IsType<UnchangedPropertyValueError>(updateEmailAddressResult.Errors.Single());
        }

        /// <summary>
        /// Tests update a user's full name.
        /// </summary>
        /// <param name="fullName">New full name.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("Harpa Stefansdottir", false)]
        [InlineData("Harpa Gunnarsson", true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public static void TryUpdateFullName_Multiple_ReturnsExpectedIsSuccess(
            string? fullName,
            bool expectedIsSuccess)
        {
            // Arrange

            User user = User
                .TryCreate(
                    "Harpa Stefansdottir",
                    "harpa.stefansdottir@example.com",
                    "123-456-7890")
                .Value;

            // Act

            Result updateFullNameResult = user.TryUpdateFullName(fullName);

            // Assert

            Assert.Equal(expectedIsSuccess, updateFullNameResult.IsSuccess);
        }

        /// <summary>
        /// Tests that an <see cref="UnchangedPropertyValueError"/> is returned in the result when
        /// an attempt to change a user's full name with the same value is made.
        /// </summary>
        [Fact]
        public static void TryUpdateFullName_UnchangedFullName_ReturnsUnchangedPropertyValueError()
        {
            // Arrange

            string fullName = "Sato Gota";

            User user = User
                .TryCreate(
                    fullName,
                    "sato.gota@example.com",
                    "123-456-7890")
                .Value;

            // Act

            Result updateFullNameResult = user.TryUpdateFullName(fullName);

            // Assert

            Assert.IsType<UnchangedPropertyValueError>(updateFullNameResult.Errors.Single());
        }

        /// <summary>
        /// Tests update a user's full name.
        /// </summary>
        /// <param name="phoneNumber">New phone number.</param>
        /// <param name="expectedIsSuccess">Expected success indicator.</param>
        [Theory]
        [InlineData("123-456-7890", false)]
        [InlineData("234-567-8901", true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, true)]
        public static void TryUpdatePhoneNumber_Multiple_ReturnsExpectedIsSuccess(
            string? phoneNumber,
            bool expectedIsSuccess)
        {
            // Arrange

            User user = User
                .TryCreate(
                    "Zhang Xia",
                    "zhang.xia@example.com",
                    "123-456-7890")
                .Value;

            // Act

            Result updateFullNameResult = user.TryUpdatePhoneNumber(phoneNumber);

            // Assert

            Assert.Equal(expectedIsSuccess, updateFullNameResult.IsSuccess);
        }

        /// <summary>
        /// Tests that an <see cref="UnchangedPropertyValueError"/> is returned in the result when
        /// an attempt to change a user's phone number with the same value is made.
        /// </summary>
        [Fact]
        public static void
            TryUpdatePhoneNumber_UnchangedPhoneNumber_ReturnsUnchangedPropertyValueError()
        {
            // Arrange

            string phoneNumber = "123-456-7890";

            User user = User
                .TryCreate(
                    "Zhang Xia",
                    "zhang.xia@example.com",
                    phoneNumber)
                .Value;

            // Act

            Result updateFullNameResult = user.TryUpdatePhoneNumber(phoneNumber);

            // Assert

            Assert.IsType<UnchangedPropertyValueError>(updateFullNameResult.Errors.Single());
        }
    }
}
