using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    public class GetSongDirectorsResponse
    {
        private GetSongDirectorsResponse(
            string message, bool isSuccess, IEnumerable<SongDirectorModel>? songDirectors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            SongDirectors = songDirectors;
        }

        public bool IsSuccess { get; }

        public string Message { get; }

        public IEnumerable<SongDirectorModel>? SongDirectors { get; }

        public static GetSongDirectorsResponse FailedToGetSongDirectors(string errorMessage) =>
            new(errorMessage, false);

        public static GetSongDirectorsResponse InvalidRequesterEmailAddress()
        {
            return new(
                "Could not get song directors because the requestor's email address was invalid.",
                false);
        }

        public static GetSongDirectorsResponse Success(IEnumerable<SongDirectorModel> songDirectors)
        {
            return new("Success.", true, songDirectors);
        }
    }
}
