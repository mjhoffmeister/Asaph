using Hydra.NET;
using System;
using System.Text.Json.Serialization;

namespace Asaph.WebApi.Models
{
    [SupportedClass(
        "SongDirector",
        Title = "Song director",
        Description = "Represents a song director.")
    ]
    [SupportedCollection("SongDirectorCollection", Title = "Song directors")]
    public record SongDirectorGrandmasterViewModel
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
