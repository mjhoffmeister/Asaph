using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Asaph.WebApi.UseCases.AddSongDirector;

/// <summary>
/// Add song director API.
/// </summary>
public class AddSongDirectorApi : IUseCaseApi
{
    /// <inheritdoc/>
    public IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<
                IAddSongDirectorBoundary<IResult>,
                AddSongDirectorApiBoundary>(factory => new(configuration))
            .AddTransient<
                IAsyncUseCaseInteractor<AddSongDirectorRequest, IResult>,
                AddSongDirectorInteractor<IResult>>();
    }

    /// <inheritdoc/>
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost(
            "/song-directors",
            [Authorize]
            async (
                SongDirectorApiModel newSongDirector,
                HttpContext http,
                IAsyncUseCaseInteractor<AddSongDirectorRequest, IResult>
                    addSongDirectorInteractor) =>
                {
                    string? requesterId = http.User.GetNameIdentifierId();

                    if (requesterId == null)
                        return Results.Unauthorized();

                    AddSongDirectorRequest addSongDirectorRequest = new(
                        requesterId,
                        newSongDirector.Name,
                        newSongDirector.EmailAddress,
                        newSongDirector.PhoneNumber,
                        newSongDirector.Rank,
                        newSongDirector.IsActive);

                    return await addSongDirectorInteractor
                        .HandleAsync(addSongDirectorRequest)
                        .ConfigureAwait(false);
                });

        return builder;
    }
}
