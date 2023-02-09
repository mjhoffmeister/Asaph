using Asaph.Core.UseCases;
using Asaph.Core.UseCases.UpdateSongDirector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Asaph.WebApi.UseCases.UpdateSongDirector;

/// <summary>
/// Update song director API.
/// </summary>
public class UpdateSongDirectorApi : IUseCaseApi
{
    /// <inheritdoc/>
    public IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<
                IUpdateSongDirectorBoundary<IResult>,
                UpdateSongDirectorApiBoundary>(factory => new(configuration))
            .AddTransient<
                IAsyncUseCaseInteractor<UpdateSongDirectorRequest, IResult>,
                UpdateSongDirectorInteractor<IResult>>();
    }

    /// <inheritdoc/>
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut(
            "/song-directors/{id}",
            [Authorize]
            async (
                string id,
                SongDirectorApiModel updatedSongDirector,
                HttpContext http,
                IAsyncUseCaseInteractor<UpdateSongDirectorRequest, IResult>
                    updateSongDirectorInteractor) =>
            {
                string? requesterId = http.User.GetNameIdentifierId();

                if (requesterId == null)
                    return Results.Unauthorized();

                UpdateSongDirectorRequest updateSongDirectorRequest = new(
                    requesterId,
                    id,
                    updatedSongDirector.Name,
                    updatedSongDirector.EmailAddress,
                    updatedSongDirector.PhoneNumber,
                    updatedSongDirector.Rank,
                    updatedSongDirector.IsActive);

                return await updateSongDirectorInteractor
                    .HandleAsync(updateSongDirectorRequest)
                    .ConfigureAwait(false);
            });

        return builder;
    }
}