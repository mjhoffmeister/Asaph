using Asaph.WebApi.GetApiDocumentation;
using Microsoft.AspNetCore.Builder;

namespace Asaph.WebApi;

/// <summary>
/// Extends <see cref="IApplicationBuilder"/>.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Adds authentication to API documentation.
    /// </summary>
    /// <param name="builder">App builder.</param>
    /// <returns><see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseSwaggerUIAuthentication(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SwaggerUIAuthenticationMiddleware>();
    }
}