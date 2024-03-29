﻿using FluentResults;
using System;

namespace Asaph.Core.Domain.SongAggregate;

/// <summary>
/// Pitch value object.
/// </summary>
public record Pitch
{
    private Pitch(Note note, int octave) => (Note, Octave) = (note, octave);

    /// <summary>
    /// Note.
    /// </summary>
    public Note Note { get; }

    /// <summary>
    /// Octave.
    /// </summary>
    public int Octave { get; }

    /// <summary>
    /// Gets the frequency of the pitch in hertz.
    /// </summary>
    /// <returns>Frequency.</returns>
    public double GetFrequency()
    {
        int midiNumber = (12 * (Octave + 1)) + Note.Number;

        // Note: 12.0 is used instead of 12 to prevent integer division
        return 440 * Math.Pow(2, (midiNumber - 69) / 12.0);
    }

    /// <summary>
    /// Tries to parse a pitch.
    /// </summary>
    /// <param name="pitchString">The sting to parse.</param>
    /// <returns>The result of the attempt.</returns>
    public static Result<Pitch> TryParse(string pitchString)
    {
        // Assume a single-digit octave at the end of the pitch string
        // For example: F♯3, C4, etc.
        string noteName = pitchString[0..^1];
        char octaveChar = pitchString[^1];

        // Try to get note and octave
        Result<Note> noteGetResult = Note.TryGetByName(noteName);
        Result<int> octaveParseResult = TryParseOctave(octaveChar);

        // Validate
        var validationResult = Result.Merge(noteGetResult, octaveParseResult);

        if (validationResult.IsFailed)
            return validationResult;

        return Result.Ok(new Pitch(noteGetResult.Value, octaveParseResult.Value));
    }

    /// <summary>
    /// Tries to parse the octave.
    /// </summary>
    /// <param name="octaveChar">Octave character to pase.</param>
    /// <returns>The result of the attempt.</returns>
    private static Result<int> TryParseOctave(char octaveChar)
    {
        if (!int.TryParse(octaveChar.ToString(), out int octave))
            return Result.Fail("Invalid octave");

        return Result.Ok(octave);
    }
}
