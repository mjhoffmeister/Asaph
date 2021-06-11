using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;

namespace Asaph.Infrastructure
{
    public class SongDirectorDocument
    {
        public bool IsActive { get; set; }

        public string? EmailAddress { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Rank { get; set; }

        public Result<SongDirector> TryGetSongDirector() => 
            SongDirector.TryCreate(FullName, EmailAddress, PhoneNumber, Rank, IsActive);

    }
}
