namespace Asaph.Core.UseCases.AddSongDirector
{
    /// <summary>
    /// Request for creating a new song director.
    /// </summary>
    public class AddSongDirectorRequest
    {
        /// <summary>
        /// Creates a new request for adding a song director.
        /// </summary>
        /// <param name="requesterRank">
        /// The rank of the person requesting the song director add.
        /// </param>
        /// <param name="fullName">Full name of the new song director.</param>
        /// <param name="emailAddress">Email address of the new song director.</param>
        /// <param name="phoneNumber">Phone number of the new song director.</param>
        /// <param name="rankName">Rank of the new song director.</param>
        /// <param name="isActive">Active indicator.</param>
        public AddSongDirectorRequest(
            string requesterId,
            string? fullName,
            string? emailAddress,
            string? phoneNumber,
            string? rankName,
            bool? isActive)
        {
            EmailAddress = emailAddress;
            FullName = fullName;
            IsActive = isActive;
            PhoneNumber = phoneNumber;
            RankName = rankName;
            RequesterId = requesterId;
        }

        /// <summary>
        /// Email address of the new song director.
        /// </summary>
        public string? EmailAddress { get; }

        /// <summary>
        /// Full name of the new song director.
        /// </summary>
        public string? FullName { get; }

        /// <summary>
        /// True if the new song director is active; false, otherwise.
        /// </summary>
        public bool? IsActive { get; }

        /// <summary>
        /// Phone number of the new song director.
        /// </summary>
        public string? PhoneNumber { get; }

        /// <summary>
        /// Rank of the new song director.
        /// </summary>
        public string? RankName { get; }

        /// <summary>
        /// The rank of the person trying to add a song director.
        /// </summary>
        public string RequesterId { get; }
    }
}
