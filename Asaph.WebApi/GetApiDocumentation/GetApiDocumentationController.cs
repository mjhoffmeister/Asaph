using Asaph.WebApi.ApiModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.WebApi.GetApiDocumentation;

/// <summary>
/// Controller for getting API documentation.
/// </summary>
[Route("api-docs/current/openapi.json")]
[ApiController]
public class GetApiDocumentationController : ControllerBase
{
    private readonly string _authorizationUrlTemplate = "{0}/{1}/{2}/oauth2/v2.0/authorize";

    // Configuration
    private readonly IConfiguration _configuration;

    private readonly string _tokenUrlTemplate = "{0}/{1}/{2}/oauth2/v2.0/token";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetApiDocumentationController"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public GetApiDocumentationController(IConfiguration configuration) =>
        _configuration = configuration;

    // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-6.0

    /// <summary>
    /// Gets the Asaph API's documentation.
    /// </summary>
    /// <returns>API documentation.</returns>
    [HttpGet]
    public IActionResult Get()
    {
        OpenApiDocument asaphDocumentation = new()
        {
            Info = new OpenApiInfo
            {
                Title = "Asaph API",
                Version = "1.0.0",
            },
            SecurityRequirements = GetOpenApiSecurityRequirements(),
            Paths = new OpenApiPaths
            {
                [$"{_configuration["BaseUri"]}/song-directors"] =
                    SongDirectorApiModel.GetOpenApiPath(User),
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["SongDirector"] = SongDirectorApiModel.GetJsonSchema(User),
                },
                SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["oauth2"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = GetAuthorizationUrl(),

                                // Colons aren't allowed in config keys, so scopes keys and values
                                // are swapped. This swaps them back.
                                Scopes = _configuration
                                    .GetSection("SwaggerUI:Scopes")
                                    .GetChildren()
                                    .ToDictionary(kv => kv.Value, kv => kv.Key),

                                TokenUrl = GetTokenUrl(),
                            },
                        },
                    },
                },
            },
        };

        string json = asaphDocumentation
            .Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);

        return Content(json, "application/json");
    }

    /// <summary>
    /// Gets the API's authorization URL.
    /// </summary>
    /// <returns>Authorization URL.</returns>
    private Uri GetAuthorizationUrl() => new(string.Format(
        _authorizationUrlTemplate,
        _configuration["AzureAdb2c:Instance"],
        _configuration["AzureAdb2c:Domain"],
        _configuration["AzureAdb2c:SignUpSignInPolicyId"]));

    /// <summary>
    /// Gets security requirements for the API.
    /// </summary>
    /// <returns>A list of <see cref="OpenApiSecurityRequirement"/>s.</returns>
    private List<OpenApiSecurityRequirement> GetOpenApiSecurityRequirements()
    {
        OpenApiSecurityRequirement[] openApiSecurityRequirements = new[]
        {
            // Adds the oath2 requirement to all operations
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2",
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header,
                    },
                    new[] { _configuration["SwaggerUI:ApiScope"] }
                },
            },
        };

        return openApiSecurityRequirements.ToList();
    }

    /// <summary>
    /// Gets the API's token URL.
    /// </summary>
    /// <returns>Token URL.</returns>
    private Uri GetTokenUrl() => new(string.Format(
        _tokenUrlTemplate,
        _configuration["AzureAdb2c:Instance"],
        _configuration["AzureAdb2c:Domain"],
        _configuration["AzureAdb2c:SignUpSignInPolicyId"]));
}
