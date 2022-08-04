using Asaph.Core.UseCases;
using Asaph.Core.UseCases.RemoveSongDirector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Asaph.WebApi.UseCases.RemoveSongDirectors;

/// <summary>
/// Get song directors API.
/// </summary>
public class RemoveSongDirectorApi : IUseCaseApi
{
    /// <inheritdoc/>
    public IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<
                IRemoveSongDirectorBoundary<IResult>,
                RemoveSongDirectorApiBoundary>(
                    factory => new(configuration))
            .AddTransient<
                IAsyncUseCaseInteractor<RemoveSongDirectorRequest, IResult>,
                RemoveSongDirectorInteractor<IResult>>();
    }

    /// <inheritdoc/>
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete(
            "/song-directors/{id}",
            [Authorize]
            async (
                string id,
                HttpContext http,
                IAsyncUseCaseInteractor<
                    RemoveSongDirectorRequest, IResult> removeSongDirectorInteractor) =>
                {
                    string? requesterId = http.User.GetNameIdentifierId();

                    if (requesterId == null || !http.User.IsGrandmasterSongDirector())
                        return Results.Unauthorized();

                    RemoveSongDirectorRequest removeSongDirectorRequest = new(requesterId, id);

                    return await removeSongDirectorInteractor
                        .HandleAsync(removeSongDirectorRequest)
                        .ConfigureAwait(false);
                });

        return builder;
    }
}
