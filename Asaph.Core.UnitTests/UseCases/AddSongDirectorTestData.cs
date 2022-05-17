using System.Collections;
using System.Collections.Generic;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test data for Add Song Director tests./>.
/// </summary>
public class AddSongDirectorInteractorTestData : IEnumerable<object?[]>
{
    /// <inheritdoc/>
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[]
        {
            "Grandmaster",
            "John Doe",
            "john.doe@example.com",
            "123-456-1234",
            null,
            true,
            "John Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            "jane.doe@example.com",
            "123-456-1234",
            "Apprentice",
            false,
            "Jane Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "John Doe",
            "john.doe@example.com",
            "123-456-1234",
            "Journeyer",
            true,
            "John Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            "jane.doe@example.com",
            "123-456-1234",
            "Master",
            true,
            "Jane Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "John Doe",
            "john.doe@example.com",
            "123-456-1234",
            "Grandmaster",
            true,
            "John Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            "jane.doe@example.com",
            null,
            null,
            true,
            "Jane Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            null,
            "john.doe@example.com",
            null,
            null,
            true,
            "Full name is required.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            null,
            null,
            null,
            true,
            "Email address is required.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            "jane.doe@example.com",
            null,
            null,
            null,
            "Jane Doe was added.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "John Doe",
            "123456789",
            null,
            null,
            true,
            "Invalid email address.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "Jane Doe",
            "jane.doe@example.com",
            "987-654-987",
            null,
            true,
            "Invalid phone number.",
        };

        yield return new object?[]
        {
            "Grandmaster",
            "John Doe",
            "john.doe@example.com",
            null,
            "Associate",
            true,
            "Invalid rank.",
        };

        yield return new object?[]
        {
            "Master",
            "Jane Doe",
            "jane.doe@example.com",
            null,
            null,
            true,
            "You don't have permission to add song directors.",
        };

        yield return new object?[]
        {
            "Journeyer",
            "John Doe",
            "John.doe@example.com",
            null,
            null,
            true,
            "You don't have permission to add song directors.",
        };

        yield return new object?[]
        {
            "Apprentice",
            "Jane Doe",
            "jane.doe@example.com",
            null,
            null,
            true,
            "You don't have permission to add song directors.",
        };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}