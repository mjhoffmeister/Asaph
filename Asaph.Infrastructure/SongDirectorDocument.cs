using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using Newtonsoft.Json;

namespace Asaph.Infrastructure
{
    public class SongDirectorDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        public string? EmailAddress { get; set; }

        public string? FullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? PhoneNumber { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Rank { get; set; }

        public Result<SongDirector> TryGetSongDirector() => 
            SongDirector.TryCreate(FullName, EmailAddress, PhoneNumber, Rank);

    }
}
