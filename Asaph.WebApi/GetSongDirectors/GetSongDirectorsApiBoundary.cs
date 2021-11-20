using Asaph.Core.UseCases.GetSongDirectors;
using Asaph.WebApi.ApiModels;
using Hydra.NET;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Asaph.WebApi.GetSongDirectors
{
    /// <summary>
    /// API boundary for the Get Song Directors use case.
    /// </summary>
    public class GetSongDirectorsApiBoundary : ApiBoundary, IGetSongDirectorBoundary<IActionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSongDirectorsApiBoundary"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public GetSongDirectorsApiBoundary(ApiBoundaryConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc/>
        public IActionResult FailedToGetSongDirectors(GetSongDirectorsResponse response) =>
            new ObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.BadGateway,
                "Bad Gateway",
                response.Message))
            {
                StatusCode = (int)HttpStatusCode.BadGateway,
            };

        /// <inheritdoc/>
        public IActionResult InvalidRequesterEmailAddress(GetSongDirectorsResponse response) =>
            new BadRequestObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                response.Message));

        /// <inheritdoc/>
        public IActionResult RequestorSongDirectorRankNotFound(GetSongDirectorsResponse response) =>
            new UnauthorizedObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                response.Message));

        /// <inheritdoc/>
        public IActionResult Success(GetSongDirectorsResponse response)
        {
            if (response.SongDirectors == null)
            {
                throw new ArgumentException("Song directors must be set for a successful Get" +
                    "Song Directors response.");
            }

            // Convert use case models to API models
            IEnumerable<SongDirectorApiModel> songDirectorApiModels = response.SongDirectors
                .Select(useCaseModel => SongDirectorApiModel
                    .RetrievedSongDirector(
                        HydraContext,
                        new Uri(ResourceBaseUri, useCaseModel.Id),
                        useCaseModel));

            return new OkObjectResult(songDirectorApiModels);
        }
    }
}
