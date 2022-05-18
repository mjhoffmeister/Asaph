using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Asaph.Core.UseCases.GetSongDirectors;
using Asaph.Core.UseCases.RemoveSongDirector;
using Asaph.WebApi.ApiBoundaries;

/// <summary>
/// Use case service collection extensions.
/// </summary>
internal static class UseCaseServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Add Song Director use case.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="resourceBaseUri">Song directors base URI.</param>
    /// <param name="hydraContextUri">Hydra context base URI.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAddSongDirectorUseCase(
        this IServiceCollection services, string resourceBaseUri, string hydraContextUri)
    {
        return services
            .AddTransient<
                IAddSongDirectorBoundary<IResult>,
                AddSongDirectorApiBoundary>(factory => new(new(hydraContextUri, resourceBaseUri)))
            .AddTransient<
                IAsyncUseCaseInteractor<AddSongDirectorRequest, IResult>,
                AddSongDirectorInteractor<IResult>>();
    }

    /// <summary>
    /// Adds the Get Song Directors use case.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="resourceBaseUri">Song directors base URI.</param>
    /// <param name="hydraContextUri">Hydra context base URI.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddGetSongDirectorsUseCase(
        this IServiceCollection services, string resourceBaseUri, string hydraContextUri)
    {
        return services
            .AddTransient<
                IGetSongDirectorsBoundary<IResult>,
                GetSongDirectorsApiBoundary>(factory => new(new(hydraContextUri, resourceBaseUri)))
            .AddTransient<
                IAsyncUseCaseInteractor<GetSongDirectorsRequest, IResult>,
                GetSongDirectorsInteractor<IResult>>();
    }

    /// <summary>
    /// Adds the Remove Song Directors use case.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="resourceBaseUri">Song directors base URI.</param>
    /// <param name="hydraContextUri">Hydra context base URI.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRemoveSongDirectorUseCase(
        this IServiceCollection services, string resourceBaseUri, string hydraContextUri)
    {
        return services
            .AddTransient<
                IRemoveSongDirectorBoundary<IResult>,
                RemoveSongDirectorApiBoundary>(
                    factory => new(new(hydraContextUri, resourceBaseUri)))
            .AddTransient<
                IAsyncUseCaseInteractor<RemoveSongDirectorRequest, IResult>,
                RemoveSongDirectorInteractor<IResult>>();
    }
}