namespace Asaph.WebApi.UseCases;

/// <summary>
/// Use case API extensions.
/// </summary>
public static class UseCaseApiExtensions
{
    // Registered use case APIs.
    private static readonly List<IUseCaseApi> _addedUseCaseApis = new();

    /// <summary>
    /// Adds APIs.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUseCaseApis(
        this IServiceCollection services, IConfiguration configuration)
    {
        IEnumerable<IUseCaseApi> apis = typeof(IUseCaseApi).Assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsAssignableTo(typeof(IUseCaseApi)))
            .Select(Activator.CreateInstance)
            .Cast<IUseCaseApi>();

        foreach (IUseCaseApi api in apis)
        {
            api.AddServices(services, configuration);
            _addedUseCaseApis.Add(api);
        }

        return services;
    }

    /// <summary>
    /// Maps API endpoints.
    /// </summary>
    /// <param name="app">Web application.</param>
    /// <returns><see cref="WebApplication"/>.</returns>
    public static WebApplication MapUseCaseApiEndpoints(this WebApplication app)
    {
        _addedUseCaseApis.ForEach(api => api.MapEndpoint(app));

        return app;
    }
}
