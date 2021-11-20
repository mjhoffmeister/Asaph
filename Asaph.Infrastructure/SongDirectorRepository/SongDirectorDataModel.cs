using Amazon.DynamoDBv2.DataModel;
using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.Infrastructure.SongDirectorRepository
{
    /// <summary>
    /// Song director data model.
    /// </summary>
    [DynamoDBTable("SongDirectors")]
    public class SongDirectorDataModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongDirectorDataModel"/> class.
        /// </summary>
        public SongDirectorDataModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongDirectorDataModel"/> class.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <param name="isActive">Is active.</param>
        public SongDirectorDataModel(
            string? id,
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool? isActive)
        {
            EmailAddress = emailAddress;
            FullName = fullName;
            Id = id;
            IsActive = isActive;
            PhoneNumber = phoneNumber;
            RankName = rankName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongDirectorDataModel"/> class.
        /// </summary>
        /// <param name="songDirector">Song director.</param>
        public SongDirectorDataModel(SongDirector songDirector)
            : this(
                  songDirector.Id,
                  songDirector.FullName,
                  songDirector.EmailAddress,
                  songDirector.PhoneNumber,
                  songDirector.Rank?.Name,
                  songDirector.IsActive)
        {
        }

        /// <summary>
        /// Id.
        /// </summary>
        [DynamoDBHashKey]
        public string? Id { get; set; }

        /// <summary>
        /// Full name.
        /// </summary>
        [DynamoDBIgnore]
        public string? FullName { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [DynamoDBIgnore]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [DynamoDBIgnore]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Rank name.
        /// </summary>
        [DynamoDBIgnore]
        public string? RankName { get; set; }

        /// <summary>
        /// Active indicator.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Tries to merge song director data models.
        /// </summary>
        /// <param name="songDirectorDataModels">The song director data models to merge.</param>
        /// <returns>The result of the merge attempt.</returns>
        public static Result<SongDirectorDataModel> TryMerge(
            IEnumerable<SongDirectorDataModel> songDirectorDataModels)
        {
            SongDirectorDataModel[] dataModelArray = songDirectorDataModels.ToArray();
            SongDirectorDataModel current = dataModelArray[0];

            for (int i = 1; i < dataModelArray.Length; i++)
            {
                SongDirectorDataModel next = dataModelArray[i];
                Result<SongDirectorDataModel> mergeResult = TryMerge(current, next);

                if (mergeResult.IsFailed)
                    return mergeResult;

                current = mergeResult.Value;
            }

            return Result.Ok(current);
        }

        /// <summary>
        /// Tries to merge two data models.
        /// </summary>
        /// <param name="first">First data model.</param>
        /// <param name="second">Second data model.</param>
        /// <returns>The result of the attempt.</returns>
        private static Result<SongDirectorDataModel> TryMerge(
            SongDirectorDataModel first, SongDirectorDataModel second)
        {
            // Return a failure result if the ids of the two data models can't be merged
            if (first.Id != second.Id)
                return Result.Fail("Song director data models with different ids can't be merged.");

            // Merge values
            Result<string?> mergeFullNameResult = TryMergeValue(first, second, m => m.FullName);

            Result<string?> mergeEmailAddressResult = TryMergeValue(
                first, second, m => m.EmailAddress);

            Result<string?> mergePhoneNumberResult = TryMergeValue(
                first, second, m => m.PhoneNumber);

            Result<string?> mergeRankNameResult = TryMergeValue(first, second, m => m.RankName);

            Result<bool?> mergeIsActiveResult = TryMergeValue(first, second, m => m.IsActive);

            // Validate merges
            Result mergeResult = Result.Merge(
                mergeEmailAddressResult,
                mergeFullNameResult,
                mergeIsActiveResult,
                mergePhoneNumberResult,
                mergeRankNameResult);

            // Return a failure result if any merges failed
            if (mergeResult.IsFailed)
                return mergeResult;

            // Return a success result if the merges succeeded
            return Result.Ok(new SongDirectorDataModel(
                first.Id,
                mergeFullNameResult.Value,
                mergeEmailAddressResult.Value,
                mergePhoneNumberResult.Value,
                mergeRankNameResult.Value,
                mergeIsActiveResult.Value));
        }

        /// <summary>
        /// Tries to merge values.
        /// </summary>
        /// <typeparam name="T">The type of the value to merge.</typeparam>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <param name="valueSelector">Value selector.</param>
        /// <returns>The result of the merge attempt.</returns>
        private static Result<T?> TryMergeValue<T>(
            SongDirectorDataModel first,
            SongDirectorDataModel second,
            Func<SongDirectorDataModel, T?> valueSelector)
        {
            T? firstValue = valueSelector(first);
            T? secondValue = valueSelector(second);

            if (firstValue == null && secondValue == null)
                return Result.Ok<T?>(default);

            if (firstValue != null && secondValue == null)
                return Result.Ok<T?>(firstValue);

            // Second value isn't null at this point, so it doesn't need to be checked
            if (firstValue == null)
                return Result.Ok(secondValue);

            return Result.Fail("Can't merge values when both are not null.");
        }
    }
}
