using Asaph.Core.UseCases;
using Asaph.Core.UseCases.GetSongDirectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Asaph.WebApi.GetSongDirectors;

/// <summary>
/// Controller for getting song directors.
/// </summary>
[Route(Routes.SongDirectors)]
[ApiController]
public class GetSongDirectorsController : ControllerBase
{
    // Interactor for the Get Song Directors use case.
    private readonly IAsyncUseCaseInteractor<
        GetSongDirectorsRequest, IActionResult> _getSongDirectorsInteractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSongDirectorsController"/> class.
    /// </summary>
    /// <param name="getSongDirectorsInteractor">
    /// Interactor for the Get Song Directors use case.
    /// </param>
    public GetSongDirectorsController(
        IAsyncUseCaseInteractor<
            GetSongDirectorsRequest, IActionResult> getSongDirectorsInteractor) =>
        _getSongDirectorsInteractor = getSongDirectorsInteractor;

    /// <summary>
    /// Gets song directors.
    /// </summary>
    /// <param name="getSongDirectorsRequest">The get song directors request.</param>
    /// <returns>The result of the operation.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAsync(GetSongDirectorsRequest getSongDirectorsRequest)
    {
        return await _getSongDirectorsInteractor
            .HandleAsync(getSongDirectorsRequest)
            .ConfigureAwait(false);
    }
}