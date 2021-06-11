using Asaph.Core.Domain.PersonAggregate;
using FluentResults;

namespace Asaph.Core.Domain.SongDirectorAggregate
{
    public class SongDirector : Person
    {
        private SongDirector(
            string fullName, string emailAddress, string? phoneNumber, Rank? rank, bool isActive) :
            base(fullName, emailAddress, phoneNumber) => (IsActive, Rank) = (isActive, rank);

        /// <summary>
        /// True if the song director is active; false, otherwise.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Rank.
        /// </summary>
        public Rank? Rank { get; }

        /// <summary>
        /// Tries to create a song director.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="isActive">True if the song director is active; false, otherwise.</param>
        /// <returns>THe result of the attempt.</returns>
        public static Result<SongDirector> TryCreate(
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool isActive)
        {
            // Try to get rank
            Result<Rank?> createRankResult = Rank.TryGetByName(rankName);

            // Validate person properties
            Result<Person> validatePersonResult = TryCreate(fullName, emailAddress, phoneNumber);

            // Merge the results into a single validation result
            Result validationResult = Result.Merge(
                createRankResult, validatePersonResult);

            // Return the failure result if the validation failed
            if (validationResult.IsFailed)
                return validationResult;

            // Return a success result with the new song director if the validation succeeded
            return Result.Ok(new SongDirector(
                fullName!, emailAddress!, phoneNumber, createRankResult.Value, isActive));
        }
    }
}
