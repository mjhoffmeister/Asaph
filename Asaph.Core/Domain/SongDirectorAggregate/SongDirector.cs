using Asaph.Core.Domain.UserAggregate;
using FluentResults;

namespace Asaph.Core.Domain.SongDirectorAggregate
{
    /// <summary>
    /// Song director entity.
    /// </summary>
    public class SongDirector : User
    {
        private SongDirector(
            string fullName,
            string emailAddress,
            string? phoneNumber,
            Rank? rank,
            bool isActive)
            : base(fullName, emailAddress, phoneNumber) => (IsActive, Rank) = (isActive, rank);

        /// <summary>
        /// True if the song director is active; false, otherwise.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Rank.
        /// </summary>
        public Rank? Rank { get; private set; }

        /// <summary>
        /// Tries to create a song director.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="isActive">True if the song director is active; false, otherwise.</param>
        /// <returns>The result of the attempt.</returns>
        public static Result<SongDirector> TryCreate(
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool? isActive)
        {
            // Try to get rank
            Result<Rank?> createRankResult = Rank.TryGetByName(rankName);

            // Validate person properties
            Result<User> validateUserResult = TryCreate(fullName, emailAddress, phoneNumber);

            // Merge the results into a single validation result
            Result validationResult = Result.Merge(
                createRankResult, validateUserResult);

            // Return the failure result if the validation failed
            if (validationResult.IsFailed)
                return validationResult;

            // Reference the valid user
            User user = validateUserResult.Value;

            // Return a success result with the new song director if the validation succeeded
            return Result.Ok(new SongDirector(
                user.FullName,
                user.EmailAddress,
                user.PhoneNumber,
                createRankResult.Value,
                isActive ?? false));
        }

        /// <summary>
        /// Tries to update the song director's active indicator.
        /// </summary>
        /// <param name="isActive">New active indicator.</param>
        /// <returns>The result of the attempt.</returns>
        public Result TryUpdateIsActive(bool? isActive)
        {
            if (isActive == null)
                return Result.Fail("Active indicator is required.");

            if (IsActive == isActive)
                return Result.Fail(new UnchangedPropertyValueError("Active indicator"));

            IsActive = isActive.Value;

            return Result.Ok();
        }

        /// <summary>
        /// Tries to update the song director's rank.
        /// </summary>
        /// <param name="rankName">New Rank name.</param>
        /// <returns>The result of the attempt.</returns>
        public Result TryUpdateRank(string? rankName)
        {
            Result<Rank?> getRankResult = Rank.TryGetByName(rankName);

            if (getRankResult.IsFailed)
                return getRankResult.ToResult();

            Rank? rank = getRankResult.Value;

            if (Rank == rank)
                return Result.Fail(new UnchangedPropertyValueError("Rank"));

            Rank = rank;

            return Result.Ok();
        }
    }
}
