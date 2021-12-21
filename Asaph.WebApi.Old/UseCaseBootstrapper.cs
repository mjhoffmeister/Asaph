using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Asaph.Core.UseCases.GetSongDirectors;
using Asaph.WebApi.AddSongDirector;
using Asaph.WebApi.GetSongDirectors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Asaph.WebApi;

/// <summary>
/// Utility class for bootstrapping use cases.
/// </summary>
public static class UseCaseBootstrapper
{
    /// <summary>
    /// Adds the Add Song Director use case.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="baseUri">Base URI of the API.</param>
    /// <param name="hydraContextUri">Hydra context URI.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAddSongDirectorUseCase(
        this IServiceCollection services, string baseUri, string hydraContextUri)
    {
        return services
            .AddTransient<
                IAddSongDirectorBoundary<IActionResult>,
                AddSongDirectorApiBoundary>(factory => new(new(hydraContextUri, baseUri)))
            .AddTransient<
                IAsyncUseCaseInteractor<AddSongDirectorRequest, IActionResult>,
                AddSongDirectorInteractor<IActionResult>>();
    }

    /// <summary>
    /// Adds the Get Song Directors use case.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="baseUri">Base URI of the API.</param>
    /// <param name="hydraContextUri">Hydra context URI.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddGetSongDirectorsUseCase(
        this IServiceCollection services, string baseUri, string hydraContextUri)
    {
        return services
            .AddTransient<
                IGetSongDirectorsBoundary<IActionResult>,
                GetSongDirectorsApiBoundary>(factory => new(new(hydraContextUri, baseUri)))
            .AddTransient<
                IAsyncUseCaseInteractor<GetSongDirectorsRequest, IActionResult>,
                GetSongDirectorsInteractor<IActionResult>>();
    }
}