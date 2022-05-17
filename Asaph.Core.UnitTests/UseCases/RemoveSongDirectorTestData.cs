using System.Collections;
using System.Collections.Generic;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test data for Remove Song Director tests.
/// </summary>
public class RemoveSongDirectorTestData : IEnumerable<object?[]>
{
    /// <inheritdoc/>
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[]
        {
            "1",
            "Grandmaster",
            "2",
            "John Doe",
            "Removed John Doe.",
        };

        yield return new object?[]
        {
            "1",
            "Grandmaster",
            "1",
            "Jane Doe",
            "You can't remove yourself. If you'd like to be discluded from song director " +
            "scheduling, set yourself as inactive.",
        };

        yield return new object?[]
        {
            "2",
            "Apprentice",
            "1",
            "Jane Doe",
            "You don't have permissions to remove song directors.",
        };

        yield return new object?[]
        {
            "2",
            "Journeyer",
            "1",
            "Jane Doe",
            "You don't have permissions to remove song directors.",
        };

        yield return new object?[]
        {
            "2",
            "Master",
            "1",
            "Jane Doe",
            "You don't have permissions to remove song directors.",
        };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
