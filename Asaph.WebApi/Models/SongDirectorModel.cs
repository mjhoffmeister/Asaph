using Hydra.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Asaph.WebApi.Models
{
    [SupportedClass(
        "doc:SongDirector",
        Title = "Song director",
        Description = "Represents a song director.")
    ]
    public record SongDirectorModel
    {
        [JsonPropertyName("@id")]
        public Uri? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; }

        [JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; }

        [SupportedProperty(
            "doc:SongDirector/rank",
            Xsd.String,
            Title = "Rank",
            Description = "The song director's rank.")
        ]
        [JsonPropertyName("rank")]
        public string? Rank { get; }

        [SupportedProperty(
            "doc:SongDirector/active",
            Xsd.Boolean,
            Title = "Active",
            Description = "Indicates whether the song director is active.")
        ]
        [JsonPropertyName("active")]
        public bool? IsActive { get; }

    
    }
}
