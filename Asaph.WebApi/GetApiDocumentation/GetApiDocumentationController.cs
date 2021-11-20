using Asaph.Core.UseCases;
using Asaph.WebApi.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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
    /// <summary>
    /// Gets the Asaph API's documentation.
    /// </summary>
    /// <returns>API documentation.</returns>
    //[Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        string baseUri = $"{Request.Scheme}://{Request.Host}";

        OpenApiDocument asaphDocumentation = new()
        {
            Info = new OpenApiInfo
            {
                Title = "Asaph API",
                Version = "1.0.0",
            },
            Paths = new OpenApiPaths
            {
                [$"{baseUri}/song-directors"] = SongDirectorApiModel.GetOpenApiPath(User),
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["SongDirector"] = SongDirectorApiModel.GetOpenApiSchema(User),
                },
            },
        };

        string json = asaphDocumentation
            .Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);

        return new JsonResult(json);
    }
}
