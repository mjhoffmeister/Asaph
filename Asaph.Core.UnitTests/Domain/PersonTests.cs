using Asaph.Core.Domain.PersonAggregate;
using FluentResults;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    public static class PersonTests
    {
        [Theory]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-1234", true)]
        [InlineData("John Doe", "john.doe@mail.com", null, true)]
        [InlineData("John Doe", "john.doe@mail", "123-456-1234", false)]
        [InlineData("John Doe", "john.doe@mail.com", "123-456-123", false)]
        [InlineData("", "john.doe@mail.com", "123-456-1234", false)]
        [InlineData(" ", "john.doe@mail.com", "123-456-1234", false)]
        public static void TryCreate_Multiple_ReturnsExpectedIsSuccess(string fullName, string emailAddress, string? phoneNumber, bool expectedIsSuccess)
        {
            // Act

            Result<Person> result = Person.TryCreate(fullName, emailAddress, phoneNumber);

            // Assert

            Assert.Equal(expectedIsSuccess, result.IsSuccess);
        }
    }
}
