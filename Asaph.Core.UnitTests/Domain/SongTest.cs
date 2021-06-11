using Asaph.Core.Domain.SongAggregate;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Asaph.Core.UnitTests.Domain
{
    public static class SongTest
    {
        [Theory]
        [InlineData("It Is Well with My Soul", "D♭", "A♭5", "D♭5", 84, "Forgiveness,Peace")]
        public static void TryCreate_Multiple_ReturnsExepectedIsSuccess(
            string name,
            string key,
            string? startingPitch,
            string? endingPitch,
            int? beatsPerMinute,
            string? themesString)
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData("A4", 440)]
        [InlineData("E♭3", 155.5635)]
        [InlineData("D5", 587.3295)]
        public static void GetFrequency_MultipleValidPitches_ReturnsExpectedFrequency(
            string validPitchString, double expectedFrequency)
        {
            // Arrange

            var pitch = Pitch.TryParse(validPitchString).Value;

            // Act

            double frequency = pitch.GetFrequency();

            // Assert

            Assert.Equal(expectedFrequency, frequency, 4);
        }

        [Theory]
        [InlineData("D3", true)]
        [InlineData("C♯4", true)]
        [InlineData("E♭5", true)]
        [InlineData("D10", false)]
        [InlineData("H♯4", false)]
        public static void TryParsePitch_Multiple_ReturnsExpectedIsSuccess(string pitchString, bool expectedIsSuccess)
        {
            // Act

            Result<Pitch> pitchParseResult = Pitch.TryParse(pitchString);

            // Assert

            Assert.Equal(expectedIsSuccess, pitchParseResult.IsSuccess);
        }
    }
}
