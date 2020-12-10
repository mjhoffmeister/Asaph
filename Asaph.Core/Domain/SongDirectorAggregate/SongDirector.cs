using Asaph.Core.Domain.PersonAggregate;
using FluentResults;

namespace Asaph.Core.Domain.SongDirectorAggregate
{
    public class SongDirector : Person
    {
        private SongDirector(string fullName, string emailAddress, string? phoneNumber, Rank? rank) :
            base(fullName, emailAddress, phoneNumber) =>
                Rank = rank;

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
        /// <returns>THe result of the attempt.</returns>
        public static Result<SongDirector> TryCreate(
            string? fullName, string? emailAddress, string? phoneNumber, string? rankName)
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
            return Result.Ok(new SongDirector(fullName!, emailAddress!, phoneNumber, createRankResult.Value));
        }
    }
}
