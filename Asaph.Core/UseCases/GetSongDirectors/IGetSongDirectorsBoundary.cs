namespace Asaph.Core.UseCases.GetSongDirectors
{
    /// <summary>
    /// Boundary for the Get Song Directors use case.
    /// </summary>
    /// <typeparam name="TOutput">Type of the output.</typeparam>
    public interface IGetSongDirectorsBoundary<out TOutput>
    {
        /// <summary>
        /// Failed to get song directors.
        /// </summary>
        /// <param name="response">Response.</param>
        /// <returns>Output.</returns>
        TOutput FailedToGetSongDirectors(GetSongDirectorsResponse response);

        /// <summary>
        /// Invalid email address.
        /// </summary>
        /// <param name="response">Response.</param>
        /// <returns>Output.</returns>
        TOutput InvalidRequesterEmailAddress(GetSongDirectorsResponse response);

        /// <summary>
        /// Requester song director rank not found.
        /// </summary>
        /// <param name="response">Response.</param>
        /// <returns>Output.</returns>
        TOutput RequesterSongDirectorRankNotFound(GetSongDirectorsResponse response);

        /// <summary>
        /// Success.
        /// </summary>
        /// <param name="response">Response.</param>
        /// <returns>Output.</returns>
        TOutput Success(GetSongDirectorsResponse response);
    }
}