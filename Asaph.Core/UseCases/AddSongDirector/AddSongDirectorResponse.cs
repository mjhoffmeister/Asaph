using System;
using System.Collections.Generic;

namespace Asaph.Core.UseCases.AddSongDirector
{
    public sealed class AddSongDirectorResponse
    {
        /// <summary>
        /// Creates a response indicating insufficient permissions to add a song director.
        /// </summary>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse InsufficientPermissions() =>
            new(null, "You don't have permission to add song directors.");

        /// <summary>
        /// Creates a new response indicating that the song director was added.
        /// </summary>
        /// <param name="id">The id of the added song director.</param>
        /// <param name="fullName">Full name of the new song director.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse SongDirectorAdded(Guid addedSongDirectorId, string fullName) =>
            new(addedSongDirectorId, $"{fullName} was added.");

        /// <summary>
        /// Creates a new response indicating that there was a validation failure.
        /// </summary>
        /// <param name="errorMessages">Validation error messages.</param>
        /// <returns>The response.</returns>
        public static AddSongDirectorResponse ValidationFailure(IEnumerable<string> errorMessages) =>
            new(null, errorMessages);

        private AddSongDirectorResponse(Guid? id, string message) : this(id, new[] { message }) { }

        private AddSongDirectorResponse(Guid? addedSongDirectorId, IEnumerable<string> messages)
        {
            AddedSongDirectorId = addedSongDirectorId;
            Messages = messages;
        }

        /// <summary>
        /// The id of the added song director.
        /// </summary>
        public Guid? AddedSongDirectorId { get; }

        /// <summary>
        /// Messages.
        /// </summary>
        public IEnumerable<string> Messages { get; }
    }
}
