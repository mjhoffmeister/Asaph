using FluentResults;

namespace Asaph.Core.Domain.SongAggregate;

/// <summary>
/// Key value object.
/// </summary>
public record Key
{
    private Key(Note note, Mode mode) => (Note, Mode) = (note, mode);

    /// <summary>
    /// Mode.
    /// </summary>
    public Mode Mode { get; }

    /// <summary>
    /// Note.
    /// </summary>
    public Note Note { get; }

    /// <summary>
    /// Tries to create a key.
    /// </summary>
    /// <param name="keyName">Key name.</param>
    /// <returns>The result of the attempt.</returns>
    public static Result<Key> TryCreate(string keyName)
    {
        string[] keyParts = keyName.Split(' ');

        if (keyParts.Length != 2)
            return Result.Fail($"{keyName} is not a valid key.");

        Result<Mode> getModeResult = Mode.TryGetByName(keyParts[1]);
        Result<Note> getNoteResult = Note.TryGetByName(keyParts[0]);

        Result getModeAndNoteResult = Result.Merge(getModeResult, getNoteResult);

        if (getModeResult.IsFailed)
            return getModeAndNoteResult;

        return Result.Ok(new Key(getNoteResult.Value, getModeResult.Value));
    }
}
