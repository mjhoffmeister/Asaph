using FluentResults;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.Core.Domain.SongAggregate;

/// <summary>
/// Song entity.
/// </summary>
public class Song
{
    // Themes
    private readonly List<string> _themes;

    private Song(
        string name,
        Key key,
        IEnumerable<string>? themes = null)
    {
        Name = name;
        Key = key;
        _themes = themes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Beats per minute.
    /// </summary>
    public int? BeatsPerMinute { get; private set; }

    /// <summary>
    /// Ending pitch.
    /// </summary>
    public Pitch? EndingPitch { get; private set; }

    /// <summary>
    /// Key.
    /// </summary>
    public Key Key { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Starting pitch.
    /// </summary>
    public Pitch? StartingPitch { get; private set; }

    /// <summary>
    /// Tempo.
    /// </summary>
    public string? Tempo => BeatsPerMinute != null ? $"{BeatsPerMinute} BPM" : null;

    /// <summary>
    /// Themes.
    /// </summary>
    public IEnumerable<string> Themes => _themes;

    /// <summary>
    /// Tries to create a song.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="keyName">Key name.</param>
    /// <param name="startingPitchString">Starting pitch string.</param>
    /// <param name="endingPitchString">Ending pitch string.</param>
    /// <param name="beatsPerMinute">Beats per minute.</param>
    /// <param name="themes">Themes.</param>
    /// <returns>The result of the attempt.</returns>
    public static Result<Song> TryCreate(
        string name,
        string keyName,
        string? startingPitchString,
        string? endingPitchString,
        int? beatsPerMinute,
        IEnumerable<string>? themes)
    {
        // Validate song name
        Result validateNameResult = Result.OkIf(
            !string.IsNullOrWhiteSpace(name), "Invalid song name.");

        // Validate key
        Result<Key> validateKeyResult = Key.TryCreate(keyName);

        // Merge required property validation results
        Result songValidationResult = Result
            .Merge(validateNameResult, validateKeyResult)
            .ToResult();

        // Return the failure response if any required properties are invalid
        if (songValidationResult.IsFailed)
            return songValidationResult;

        // Create a new song
        Song song = new(name, validateKeyResult.Value, themes);

        // Try to get starting pitch, ending pitch, and beats-per-minute
        Result setStartingPitchResult = song.TryUpdateStartingPitch(startingPitchString);

        Result setEndingPitchResult = song.TryUpdateEndingPitch(endingPitchString);

        Result setBeatsPerMinuteValidationResult = song.TryUpdateBeatsPerMinute(beatsPerMinute);

        // Return the creation result
        return Result
            .Ok(song)
            .WithErrors(setStartingPitchResult.Errors)
            .WithErrors(setEndingPitchResult.Errors)
            .WithErrors(setBeatsPerMinuteValidationResult.Errors);
    }

    /// <summary>
    /// Tries to update beats-per-minute.
    /// </summary>
    /// <param name="beatsPerMinute">Beat-per-minute.</param>
    /// <returns>The result of the attempt.</returns>
    public Result TryUpdateBeatsPerMinute(int? beatsPerMinute)
    {
        if (beatsPerMinute == null)
        {
            BeatsPerMinute = null;
            return Result.Ok();
        }

        Result beatsPerMinuteValidationResult = ValidateBeatsPerMinute(beatsPerMinute.Value);

        if (beatsPerMinuteValidationResult.IsFailed)
            return beatsPerMinuteValidationResult;

        BeatsPerMinute = beatsPerMinute;

        return Result.Ok();
    }

    /// <summary>
    /// Tries to update ending pitch.
    /// </summary>
    /// <param name="endingPitchString">Ending pitch string.</param>
    /// <returns>The result of the attempt.</returns>
    public Result TryUpdateEndingPitch(string? endingPitchString)
    {
        if (endingPitchString == null)
        {
            EndingPitch = null;
            return Result.Ok();
        }

        Result<Pitch> validateEndingPitchResult = Pitch.TryParse(endingPitchString);

        if (validateEndingPitchResult.IsFailed)
            return validateEndingPitchResult.ToResult();

        EndingPitch = validateEndingPitchResult.Value;

        return Result.Ok();
    }

    /// <summary>
    /// Tries to update starting pitch.
    /// </summary>
    /// <param name="startingPitchString">Starting pitch string.</param>
    /// <returns>The result of the attempt.</returns>
    public Result TryUpdateStartingPitch(string? startingPitchString)
    {
        if (startingPitchString == null)
        {
            StartingPitch = null;
            return Result.Ok();
        }

        Result<Pitch> validateStartingPitchResult = Pitch.TryParse(startingPitchString);

        if (validateStartingPitchResult.IsFailed)
            return validateStartingPitchResult.ToResult();

        StartingPitch = validateStartingPitchResult.Value;

        return Result.Ok();
    }

    /// <summary>
    /// Validates beats-per-minute.
    /// </summary>
    /// <param name="beatsPerMinute">Beats-per-minute.</param>
    /// <returns>The validation result.</returns>
    private Result ValidateBeatsPerMinute(int beatsPerMinute) => Result.OkIf(
        beatsPerMinute >= 30 && beatsPerMinute <= 220,
        $"{beatsPerMinute} BPM is not a valid tempo");
}