using Microsoft.Identity.Web.Resource;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Security.Claims;

/// <summary>
/// Assists with building API documentation.
/// </summary>
internal static class ApiDocumentationBuilder
{
    /// <summary>
    /// Gets OpenAPI documentation for the Asaph API.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="requester">Requester.</param>
    /// <returns><see cref="OpenApiDocument"/>.</returns>
    public static OpenApiDocument GetAsaphOpenApiDocument(
        IConfiguration configuration,
        ClaimsPrincipal requester)
    {
        // Get Azure AD B2C configuration
        IConfiguration azureAdb2cConfiguration = configuration.GetSection("AzureAdb2c");

        // Get Swagger UI configuration
        IConfiguration swaggerUIConfiguration = configuration.GetSection("SwaggerUI");

        // Return the OpenApiDocument for the Asaph API
        return new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Asaph API",
                Version = "1.0.0",
            },
            SecurityRequirements = GetOpenApiSecurityRequirements(swaggerUIConfiguration),
            Paths = new OpenApiPaths
            {
                ["/song-directors"] =
                    SongDirectorApiModel.GetOpenApiPath(requester),
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["SongDirector"] = SongDirectorApiModel.GetOpenApiSchema(requester),
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
                                AuthorizationUrl = GetAuthorizationUrl(azureAdb2cConfiguration),

                                // Colons aren't allowed in config keys, so scopes keys and values
                                // are swapped. This swaps them back.
                                Scopes = swaggerUIConfiguration
                                    .GetSection("Scopes")
                                    .GetChildren()
                                    .Where(section => section.Value != null)
                                    .ToDictionary(
                                        section => section.Value!, section => section.Key),

                                TokenUrl = GetTokenUrl(azureAdb2cConfiguration),
                            },
                        },
                    },
                },
            },
        };
    }

    /// <summary>
    /// Gets the API's authorization URL.
    /// </summary>
    /// <param name="azureAdb2cConfiguration">Azure AD B2C configuration.</param>
    /// <returns>Authorization URL.</returns>
    private static Uri GetAuthorizationUrl(IConfiguration azureAdb2cConfiguration)
    {
        string? authorizationUrlTemplate = azureAdb2cConfiguration["AuthorizationUrlTemplate"];

        if (authorizationUrlTemplate == null)
        {
            throw new ConfigurationErrorsException(
                "Missing Azure AD B2C authorization URL template configuration.");
        }

        return new(string.Format(
            authorizationUrlTemplate,
            azureAdb2cConfiguration["Instance"],
            azureAdb2cConfiguration["Domain"],
            azureAdb2cConfiguration["SignUpSignInPolicyId"]));
    }

    /// <summary>
    /// Gets security requirements for the API.
    /// </summary>
    /// <param name="swaggerUIConfiguration">Swagger UI configuration.</param>
    /// <returns>A list of <see cref="OpenApiSecurityRequirement"/>s.</returns>
    private static List<OpenApiSecurityRequirement> GetOpenApiSecurityRequirements(
        IConfiguration swaggerUIConfiguration)
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
                    new[] { swaggerUIConfiguration["ApiScope"] }
                },
            },
        };

        return openApiSecurityRequirements.ToList();
    }

    /// <summary>
    /// Gets the API's token URL.
    /// </summary>
    /// <param name="azureAdb2cConfiguration">Azure AD B2C configuration.</param>
    /// <returns>Token URL.</returns>
    private static Uri GetTokenUrl(IConfiguration azureAdb2cConfiguration)
    {
        string? tokenUrlTemplate = azureAdb2cConfiguration["TokenUrlTemplate"];

        if (tokenUrlTemplate == null)
        {
            throw new ConfigurationErrorsException(
                "Missing Azure AD B2C token URL template configuration.");
        }

        return new(string.Format(
          tokenUrlTemplate,
          azureAdb2cConfiguration["Instance"],
          azureAdb2cConfiguration["Domain"],
          azureAdb2cConfiguration["SignUpSignInPolicyId"]));
    }
}