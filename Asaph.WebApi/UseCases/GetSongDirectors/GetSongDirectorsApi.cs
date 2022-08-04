using Asaph.Core.UseCases;
using Asaph.Core.UseCases.GetSongDirectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Asaph.WebApi.UseCases.GetSongDirectors;

/// <summary>
/// Get song directors API.
/// </summary>
public class GetSongDirectorsApi : IUseCaseApi
{
    /// <inheritdoc/>
    public IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<
                IGetSongDirectorsBoundary<IResult>,
                GetSongDirectorsApiBoundary>(factory => new(configuration))
            .AddTransient<
                IAsyncUseCaseInteractor<GetSongDirectorsRequest, IResult>,
                GetSongDirectorsInteractor<IResult>>();
    }

    /// <inheritdoc/>
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/song-directors",
            [Authorize]
            async (
            HttpContext http,
            IAsyncUseCaseInteractor<
                GetSongDirectorsRequest, IResult> getSongDirectorsInteractor) =>
            {
                    string? requesterId = http.User.GetNameIdentifierId();

                    if (requesterId == null)
                        return Results.Unauthorized();

                    GetSongDirectorsRequest getSongDirectorsRequest = new(requesterId);

                    return await getSongDirectorsInteractor
                        .HandleAsync(getSongDirectorsRequest)
                        .ConfigureAwait(false);
                });

        return builder;
    }
}
