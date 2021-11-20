using Asaph.Core.UseCases.AddSongDirector;
using Asaph.WebApi.ApiModels;
using Hydra.NET;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Asaph.WebApi.AddSongDirector
{
    /// <summary>
    /// API boundary for the Add Song Director use case.
    /// </summary>
    public class AddSongDirectorApiBoundary : ApiBoundary, IAddSongDirectorBoundary<IActionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongDirectorApiBoundary"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public AddSongDirectorApiBoundary(ApiBoundaryConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc/>
        public IActionResult InsufficientPermissions(AddSongDirectorResponse response) =>
            new UnauthorizedObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.Unauthorized,
                nameof(HttpStatusCode.Unauthorized),
                string.Join(Environment.NewLine, response.Messages)));

        /// <inheritdoc/>
        public IActionResult RequesterRankNotFound(AddSongDirectorResponse response) =>
            new UnauthorizedObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.Unauthorized,
                nameof(HttpStatusCode.Unauthorized),
                string.Join(Environment.NewLine, response.Messages)));

        /// <inheritdoc/>
        public IActionResult SongDirectorAdded(AddSongDirectorResponse response)
        {
            // Validate song director id
            if (response.AddedSongDirector?.Id == null)
                throw new ArgumentException("Id must be set for an added song director.");

            // Get the URL for the added song director
            string addedSongDirectorUrl = new Uri(
                ResourceBaseUri, response.AddedSongDirector.Id)
                .ToString();

            // Create a response object for the added song director
            SongDirectorApiModel addSongDirectorResponse = SongDirectorApiModel
                .AddedSongDirector(addedSongDirectorUrl);

            // Return the IActionResult representing the add
            return new CreatedResult(addedSongDirectorUrl, addSongDirectorResponse);
        }

        /// <inheritdoc/>
        public IActionResult SongDirectorAddFailed(AddSongDirectorResponse response) =>
            new ObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.BadGateway,
                "Bad Gateway",
                string.Join(Environment.NewLine, response.Messages)))
            {
                StatusCode = (int)HttpStatusCode.BadGateway,
            };

        /// <inheritdoc/>
        public IActionResult ValidationFailure(AddSongDirectorResponse response) =>
            new BadRequestObjectResult(new Status(
                HydraContext,
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                string.Join(Environment.NewLine, response.Messages)));
    }
}
