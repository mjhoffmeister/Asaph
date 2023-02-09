using Asaph.Core.Domain.SongDirectorAggregate;
using System.Collections;
using System.Collections.Generic;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test data for Update Song Director tests.
/// </summary>
public class UpdateSongDirectorTestData : IEnumerable<object?[]>
{
    /// <inheritdoc/>
    public IEnumerator<object?[]> GetEnumerator()
    {
        // Invalid request due to missing requester id
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester(null, null)
            .Request(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .ExpectedMessage("Requester id is required.")
            .Build();

        // Unauthorized
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000003", "Journeyer")
            .Request(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "234-567-9012",
                "Master",
                true)
            .ExpectedMessage("You don't have permission to update Panashe Mutsipa.")
            .Build();

        // Invalid update (email address)
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000001", "Grandmaster")
            .Request(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "foobar",
                "123-456-7890",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .ExpectedMessage("Invalid email address.")
            .Build();

        // Valid update by grandmaster (phone number)
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000001", "Grandmaster")
            .Request(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "234-567-8901",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .ExpectedMessage("Updated Panashe Mutsipa.")
            .Build();

        // Valid update by self (phone number)
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000002", "Journeyer")
            .Request(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "234-567-8901",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000002",
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .ExpectedMessage("Updated Panashe Mutsipa.")
            .Build();

        // Invalid grandmaster demotion
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000001", "Grandmaster")
            .Request(
                "00000000-0000-0000-0000-000000000001",
                "Jane Doe",
                "jane.doe@example.com",
                "789-345-7898",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000001",
                "Jane Doe",
                "jane.doe@example.com",
                "789-345-7898",
                "Grandmaster",
                true)
            .ExistingSongDirectors(GetSongDirectors(withTwoGrandmasters: false))
            .ExpectedMessage(
                "You must promote another song director to grandmaster before demoting yourself.")
            .Build();

        // Valid grandmaster demotion
        yield return UpdateSongDirectorTestCaseBuilder
            .Requester("00000000-0000-0000-0000-000000000001", "Grandmaster")
            .Request(
                "00000000-0000-0000-0000-000000000001",
                "Jane Doe",
                "jane.doe@example.com",
                "789-345-7898",
                "Master",
                true)
            .SongDirectorToUpdate(
                "00000000-0000-0000-0000-000000000001",
                "Jane Doe",
                "jane.doe@example.com",
                "789-345-7898",
                "Grandmaster",
                true)
            .ExistingSongDirectors(GetSongDirectors(withTwoGrandmasters: true))
            .ExpectedMessage("Updated Jane Doe.")
            .Build();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets song directors for the test.
    /// </summary>
    /// <param name="withTwoGrandmasters">If true, includes an additional grandmaster.</param>
    /// <returns>Song directors.</returns>
    private IEnumerable<SongDirector> GetSongDirectors(bool withTwoGrandmasters)
    {
        SongDirector harpa = SongDirector
            .TryCreate(
                "Harpa Stefansdottir",
                "harpa.stefansdottir@example.com",
                "234-123-2345",
                "Journeyer",
                true)
            .Value;

        harpa.UpdateId("00000000-0000-0000-0000-000000000003");

        yield return harpa;

        SongDirector jane = SongDirector
            .TryCreate(
                "Jane Doe",
                "jane.doe@example.com",
                "789-345-7898",
                "Grandmaster",
                true)
            .Value;

        jane.UpdateId("00000000-0000-0000-0000-000000000001");

        yield return jane;

        SongDirector panashe = SongDirector
            .TryCreate(
                "Panashe Mutsipa",
                "panashe.mutsipa@example.com",
                "123-456-7890",
                "Master",
                true)
            .Value;

        panashe.UpdateId("00000000-0000-0000-0000-000000000002");

        yield return panashe;

        if (withTwoGrandmasters)
        {
            SongDirector sato = SongDirector
            .TryCreate(
                "Sato Gota",
                "sato.gota@example.com",
                "457-294-2847",
                "Grandmaster",
                true)
            .Value;

            sato.UpdateId("00000000-0000-0000-0000-000000000004");

            yield return sato;
        }
    }
}
