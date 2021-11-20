using Asaph.Core.Domain.SongDirectorAggregate;
using System;

namespace Asaph.Core.UseCases
{
    /// <summary>
    /// Song director use case model.
    /// </summary>
    public class SongDirectorUseCaseModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongDirectorUseCaseModel"/> class.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="name">Name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="isActive">Active indicator.</param>
        /// <param name="rank">Rank.</param>
        /// <param name="isDeletable">Deletable indicator.</param>
        /// <param name="isEditable">Editable indicator.</param>
        private SongDirectorUseCaseModel(
            string id,
            string name,
            string emailAddress,
            string? phoneNumber,
            bool isActive,
            string? rank,
            bool isDeletable,
            bool isEditable)
        {
            EmailAddress = emailAddress;
            Id = id;
            IsActive = isActive;
            IsDeletable = isDeletable;
            IsEditable = isEditable;
            Name = name;
            PhoneNumber = phoneNumber;
            Rank = rank;
        }

        /// <summary>
        /// Email address.
        /// </summary>
        public string EmailAddress { get; }

        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Active indicator.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Deletable indicator.
        /// </summary>
        public bool IsDeletable { get; }

        /// <summary>
        /// Editable indicator.
        /// </summary>
        public bool IsEditable { get; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string? PhoneNumber { get; }

        /// <summary>
        /// Rank.
        /// </summary>
        public string? Rank { get; }

        /// <summary>
        /// Creates a read-only song director model.
        /// </summary>
        /// <param name="songDirector">The song director to convert.</param>
        /// <returns><see cref="SongDirectorUseCaseModel"/>.</returns>
        public static SongDirectorUseCaseModel ReadOnly(SongDirector songDirector)
        {
            ValidateId(songDirector);

            return new(
                songDirector.Id!,
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                rank: null,
                isDeletable: false,
                isEditable: false);
        }

        /// <summary>
        /// Creates a song director model representing the requester.
        /// </summary>
        /// <param name="songDirector">The song director to convert.</param>
        /// <param name="rank">Rank.</param>
        /// <returns><see cref="SongDirectorUseCaseModel"/>.</returns>
        public static SongDirectorUseCaseModel Self(SongDirector songDirector, Rank? rank)
        {
            ValidateId(songDirector);

            bool isGrandmaster = rank == Domain.SongDirectorAggregate.Rank.Grandmaster;

            return new(
                songDirector.Id!,
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                rank: isGrandmaster ? songDirector.Rank?.Name : null,
                isDeletable: false,
                isEditable: true);
        }

        /// <summary>
        /// Creates a grandmaster view of a song director model.
        /// </summary>
        /// <param name="songDirector">The song director to convert.</param>
        /// <returns><see cref="SongDirectorUseCaseModel"/>.</returns>
        public static SongDirectorUseCaseModel GrandmasterView(SongDirector songDirector)
        {
            ValidateId(songDirector);

            return new(
                songDirector.Id!,
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                songDirector.Rank?.Name,
                isDeletable: true,
                isEditable: true);
        }

        /// <summary>
        /// Validates the song director's id.
        /// </summary>
        /// <param name="songDirector">The song director to validate.</param>
        /// <exception cref="ArgumentException">Thrown if id is null.</exception>
        private static void ValidateId(SongDirector songDirector)
        {
            if (songDirector.Id == null)
                throw new ArgumentException("Invalid song director; id is not set.");
        }
    }
}
