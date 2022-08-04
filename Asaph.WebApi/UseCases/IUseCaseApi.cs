namespace Asaph.WebApi.UseCases;

/// <summary>
/// Use case API interface.
/// </summary>
public interface IUseCaseApi
{
    /// <summary>
    /// Adds services for the use case API.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Maps an enpoint for a use case API.
    /// </summary>
    /// <param name="builder">Endpoint builder.</param>
    /// <returns><see cref="IEndpointRouteBuilder"/>.</returns>
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
