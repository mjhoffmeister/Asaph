using System;
using System.Collections.Generic;

namespace Asaph.Core.UseCases.AddSongDirector
{
    /// <summary>
    /// Response for the Add Song Directors use case.
    /// </summary>
    public sealed class AddSongDirectorResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongDirectorResponse"/> class.
        /// </summary>
        /// <param name="addedSongDirector">The added song director.</param>
        /// <param name="message">Message.</param>
        private AddSongDirectorResponse(
            SongDirectorUseCaseModel? addedSongDirector, string message)
            : this(addedSongDirector, new[] { message })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongDirectorResponse"/> class.
        /// </summary>
        /// <param name="addedSongDirectorUser">The added song director.</param>
        /// <param name="messages">Messages.</param>
        private AddSongDirectorResponse(
            SongDirectorUseCaseModel? addedSongDirectorUser, IEnumerable<string> messages)
        {
            AddedSongDirector = addedSongDirectorUser;
            Messages = messages;
        }

        /// <summary>
        /// The id of the added song director.
        /// </summary>
        public SongDirectorUseCaseModel? AddedSongDirector { get; }

        /// <summary>
        /// Messages.
        /// </summary>
        public IEnumerable<string> Messages { get; }

        /// <summary>
        /// Creates a response indicating insufficient permissions to add a song director.
        /// </summary>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse InsufficientPermissions() =>
            new(null, "You don't have permission to add song directors.");

        /// <summary>
        /// Creates a response indicating that the requester's rank couldn't be found.
        /// </summary>
        /// <param name="requesterId">Requester id.</param>
        /// <param name="message">Message providing additional details.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse RequesterRankNotFound(
            string requesterId, string message)
        {
            string requesterRankNotFoundMessage = $"No rank found for user {requesterId}, so " +
                $"permission for adding song directors couldn't be checked. Details: {message}";

            return new(null, requesterRankNotFoundMessage);
        }

        /// <summary>
        /// Creates a new response indicating that the song director was added.
        /// </summary>
        /// <param name="addedSongDirector">The added song director.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse SongDirectorAdded(
            SongDirectorUseCaseModel addedSongDirector) =>
                new(addedSongDirector, $"{addedSongDirector.Name} was added.");

        /// <summary>
        /// Creates a new response indicating that the song director add failed.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse SongDirectorAddFailed(string message)
        {
            string songDirectorAddFailedMessage = "The request to add the song director was " +
                $"valid, but an infrastucture issue prevented the add. Details: {message}";

            return new(null, songDirectorAddFailedMessage);
        }

        /// <summary>
        /// Creates a new response indicating that there was a validation failure.
        /// </summary>
        /// <param name="errorMessages">Validation error messages.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse ValidationFailure(
            IEnumerable<string> errorMessages) =>
                new(null, errorMessages);
    }
}
