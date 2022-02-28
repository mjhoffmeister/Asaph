using Asaph.Core.Domain.SongAggregate;
using FluentResults;
using System;
using System.Collections.Generic;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    /// <summary>
    /// Tests the Song entity.
    /// </summary>
    public static class SongTests
    {
        /// <summary>
        /// Tests creating songs.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="keyString">Key.</param>
        /// <param name="startingPitch">Starting pitch.</param>
        /// <param name="endingPitch">Ending pitch.</param>
        /// <param name="beatsPerMinute">Beats per minute.</param>
        /// <param name="themesString">Themes.</param>
        [Theory]
        [InlineData("It Is Well with My Soul", "D♭ major", "A♭5", "D♭5", 84, "Forgiveness,Peace")]
        public static void TryCreate_Multiple_ReturnsExpectedIsSuccess(
            string name,
            string keyString,
            string? startingPitch,
            string? endingPitch,
            int? beatsPerMinute,
            string? themesString)
        {
            // Arrange

            IEnumerable<string>? themes = themesString?.Split(',');

            // Act

            Result<Song> createSongResult = Song.TryCreate(
                name, keyString, startingPitch, endingPitch, beatsPerMinute, themes);

            // Assert

            Assert.True(createSongResult.IsSuccess);
        }

        /// <summary>
        /// Tests getting frequencies from pitches.
        /// </summary>
        /// <param name="validPitchString">A valid pitch string.</param>
        /// <param name="expectedFrequency">The expected frequency.</param>
        [Theory]
        [InlineData("A4", 440)]
        [InlineData("E♭3", 155.5635)]
        [InlineData("D5", 587.3295)]
        public static void GetFrequency_MultipleValidPitches_ReturnsExpectedFrequency(
            string validPitchString, double expectedFrequency)
        {
            // Arrange

            Pitch? pitch = Pitch.TryParse(validPitchString).Value;

            // Act

            double frequency = pitch.GetFrequency();

            // Assert

            Assert.Equal(expectedFrequency, frequency, 4);
        }

        /// <summary>
        /// Tests parsing pitches.
        /// </summary>
        /// <param name="pitchString">Pitch string.</param>
        /// <param name="expectedIsSuccess">Excpected success indicator.</param>
        [Theory]
        [InlineData("D3", true)]
        [InlineData("C♯4", true)]
        [InlineData("E♭5", true)]
        [InlineData("D10", false)]
        [InlineData("H♯4", false)]
        public static void TryParsePitch_Multiple_ReturnsExpectedIsSuccess(
            string pitchString, bool expectedIsSuccess)
        {
            // Act

            Result<Pitch> pitchParseResult = Pitch.TryParse(pitchString);

            // Assert

            Assert.Equal(expectedIsSuccess, pitchParseResult.IsSuccess);
        }
    }
}
