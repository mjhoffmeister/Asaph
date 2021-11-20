using System.Collections.Generic;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    /// <summary>
    /// Response for the Get Song Directors use case.
    /// </summary>
    public class GetSongDirectorsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSongDirectorsResponse"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="songDirectors">Song directors.</param>
        private GetSongDirectorsResponse(
            string message,
            IEnumerable<SongDirectorUseCaseModel>? songDirectors = null)
        {
            Message = message;
            SongDirectors = songDirectors;
        }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Song directors.
        /// </summary>
        public IEnumerable<SongDirectorUseCaseModel>? SongDirectors { get; }

        /// <summary>
        /// Creates a response indicating that song directors couldn't be retrieved.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <returns><see cref="GetSongDirectorsResponse"/>.</returns>
        public static GetSongDirectorsResponse FailedToGetSongDirectors(string errorMessage) =>
            new(errorMessage);

        /// <summary>
        /// Creates a success response with the retrieved song directors.
        /// </summary>
        /// <param name="songDirectors">Song directors.</param>
        /// <returns><see cref="GetSongDirectorsResponse"/>.</returns>
        public static GetSongDirectorsResponse Success(
            IEnumerable<SongDirectorUseCaseModel> songDirectors)
        {
            return new("Success.", songDirectors);
        }
    }
}
