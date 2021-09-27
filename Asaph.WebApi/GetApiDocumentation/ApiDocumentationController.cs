using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.WebApi.Models;
using Hydra.NET;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Asaph.WebApi.GetApiDocumentation
{
    [Route("doc")]
    [ApiController]
    public class ApiDocumentationController : ControllerBase
    {
        private readonly string _contextPrefix = "doc";

        public ApiDocumentation Get()
        {
            // Get the base URL of the API
            Uri baseUrl = new($"{Request.Scheme}://{Request.Host}{Request.PathBase}");

            // Initialize API documentation
            ApiDocumentation apiDocumentation = new(new(baseUrl, _contextPrefix), _contextPrefix);

            // Add supported classes
            apiDocumentation
                .AddSupportedClass<SongDirectorGrandmasterViewModel>(new("SongDirector", 
                    new PropertyShape("SongDirector/rank", Rank.Enumerate().Select(r => r.Name))));
            
            return apiDocumentation;
        }
    }
}
