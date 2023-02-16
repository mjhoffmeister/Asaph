using Asaph.Core.UseCases;
using Hydra.NET;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;

/// <summary>
/// Song director API model.
/// </summary>
internal class SongDirectorApiModel
{
    /// <summary>
    /// Context.
    /// </summary>
    [JsonPropertyName("@context")]
    public Context? Context { get; set; }

    /// <summary>
    /// Id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Rank.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rank")]
    public string? Rank { get; set; }

    /// <summary>
    /// Active indicator.
    /// </summary>
    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Operations.
    /// </summary>
    [JsonPropertyName("operation")]
    public IEnumerable<Operation>? Operations { get; set; }

    /// <summary>
    /// Creates a new song director API model representing a song director that was added.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns><see cref="SongDirectorApiModel"/>.</returns>
    public static SongDirectorApiModel AddedSongDirector(string id)
    {
        return new SongDirectorApiModel
        {
            Id = id,
            Operations = new Operation[]
            {
                new(Method.Delete),
                new(Method.Get),
                new(Method.Put),
            },
        };
    }

    /// <summary>
    /// Gets the OpenAPI path for song directors.
    /// </summary>
    /// <param name="user">The user making the request.</param>
    /// <returns><see cref="OpenApiPathItem"/>.</returns>
    public static OpenApiPathItem GetOpenApiPath(ClaimsPrincipal user)
    {
        OpenApiPathItem songDirectorsPath = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "x-icon-hint", new OpenApiString("People") },
                { "x-is-nav-menu-link", new OpenApiBoolean(true) },
            },
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Get] = new()
                {
                    Description = "Gets all song directors.",
                    Responses = new()
                    {
                        ["200"] = new()
                        {
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/ld+json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Id = "SongDirector",
                                                Type = ReferenceType.Schema,
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                    Tags = GetOpenApiTags(),
                },
            },
            Summary = "Song directors",
        };

        if (user.IsGrandmasterSongDirector())
        {
            songDirectorsPath.Operations.Add(OperationType.Post, new OpenApiOperation
            {
                Description = "Adds a song director.",
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "SongDirector",
                                    Type = ReferenceType.Schema,
                                },
                            },
                        },
                    },
                },
                Responses = new()
                {
                    ["201"] = new()
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/ld+json"] = new()
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "SongDirector",
                                        Type = ReferenceType.Schema,
                                    },
                                },
                            },
                        },
                    },
                },
                Tags = GetOpenApiTags(),
            });
        }

        return songDirectorsPath;
    }

    /// <summary>
    /// Gets the OpenAPI schema for a song director.
    /// </summary>
    /// <param name="user">User making the request.</param>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema(ClaimsPrincipal user)
    {
        // Initialize song director properties
        Dictionary<string, OpenApiSchema> songDirectorProperties = new()
        {
            ["name"] = new OpenApiSchema
            {
                Title = "Name",
                Type = "string",
            },
            ["emailAddress"] = new OpenApiSchema
            {
                Title = "Email address",
                Type = "string",
            },
            ["phoneNumber"] = new OpenApiSchema
            {
                Title = "Phone number",
                Type = "string",
            },
        };

        // Add rank if the user is a grandmaster song director
        if (user.IsGrandmasterSongDirector())
        {
            songDirectorProperties.Add("rank", new OpenApiSchema
            {
                Title = "Rank",
                Type = "string",
                Enum = new List<IOpenApiAny>(
                    Roles.GetSongDirectorRankNames().Select(rank => new OpenApiString(rank))),
            });
        }

        // Add the remaining properties
        songDirectorProperties.Add("isActive", new OpenApiSchema
        {
            Title = "Active",
            Type = "boolean",
        });

        // Create and return the song director schema
        return new()
        {
            Required = new SortedSet<string>(new[]
            {
                "name",
                "emailAddress",
                "isActive",
            }),
            Properties = songDirectorProperties,
        };
    }

    /// <summary>
    /// Creates a new song director API model representing a retrieved song director.
    /// </summary>
    /// <param name="hydraContext">Hydra context.</param>
    /// <param name="songDirectorBaseUri">Song director base URI.</param>
    /// <param name="useCaseModel">Use case model.</param>
    /// <returns><see cref="SongDirectorApiModel"/>.</returns>
    public static SongDirectorApiModel RetrievedSongDirector(
        Context hydraContext, Uri songDirectorBaseUri, SongDirectorUseCaseModel useCaseModel)
    {
        return new SongDirectorApiModel
        {
            Context = hydraContext,
            EmailAddress = useCaseModel.EmailAddress,
            Id = new Uri(songDirectorBaseUri, useCaseModel.Id).ToString(),
            IsActive = useCaseModel.IsActive,
            Name = useCaseModel.Name,
            Operations = GetOperations(useCaseModel),
            PhoneNumber = useCaseModel.PhoneNumber?.Insert(3, "-")?.Insert(7, "-"),
            Rank = useCaseModel.Rank,
        };
    }

    /// <summary>
    /// Gets OpenAPI tags for the song directors resource.
    /// </summary>
    /// <returns>A list of <see cref="OpenApiTag"/>s.</returns>
    private static List<OpenApiTag> GetOpenApiTags()
    {
        OpenApiTag[] tags = new[]
        {
            new OpenApiTag { Name = "Song directors" },
        };

        return tags.ToList();
    }

    /// <summary>
    /// Gets operations for a song director use case model.
    /// </summary>
    /// <param name="useCaseModel">Use case model.</param>
    /// <returns>Operations.</returns>
    private static IEnumerable<Operation> GetOperations(SongDirectorUseCaseModel useCaseModel)
    {
        if (useCaseModel.IsEditable)
            yield return new(Method.Put);

        if (useCaseModel.IsDeletable)
            yield return new(Method.Delete);

        yield return new(Method.Get);
    }
}