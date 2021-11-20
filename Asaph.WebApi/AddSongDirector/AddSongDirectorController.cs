using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Asaph.WebApi.AddSongDirector
{
    /// <summary>
    /// Controller for adding a song director.
    /// </summary>
    [Route(Routes.SongDirectors)]
    [ApiController]
    public class AddSongDirectorController : ControllerBase
    {
        // Interactor for the Add Song Director use case.
        private readonly IAsyncUseCaseInteractor<
            AddSongDirectorRequest, IActionResult> _addSongDirectorInteractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongDirectorController"/> class.
        /// </summary>
        /// <param name="addSongDirectorInteractor">
        /// Interactor for the Add Song Director use case.
        /// </param>
        public AddSongDirectorController(
            IAsyncUseCaseInteractor<
                AddSongDirectorRequest, IActionResult> addSongDirectorInteractor) =>
            _addSongDirectorInteractor = addSongDirectorInteractor;

        /// <summary>
        /// Adds a song director.
        /// </summary>
        /// <param name="addSongDirectorRequest">The add song director request.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPost]
        [Authorize(Policy = "GrandmasterOnly")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Post(AddSongDirectorRequest addSongDirectorRequest)
        {
            return await _addSongDirectorInteractor
                .HandleAsync(addSongDirectorRequest)
                .ConfigureAwait(false);
        }
    }
}
