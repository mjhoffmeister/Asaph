using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Asaph.WebApi;

/// <summary>
/// Middleware for API documentation authentication.
/// </summary>
public class SwaggerUIAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerUIAuthenticationMiddleware"/>
    /// class.
    /// </summary>
    /// <param name="next">Next request delegate.</param>
    public SwaggerUIAuthenticationMiddleware(RequestDelegate next) => _next = next;

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    /// <returns>The result of the async operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            if (!context.User.Identity?.IsAuthenticated == true)
            {
                await context
                    .ChallengeAsync(
                        "oidc",
                        new AuthenticationProperties
                        {
                            RedirectUri = "https://asaphworship.b2clogin.com/asaphworship.onmicrosoft.com/oauth2/authresp",
                        })
                    .ConfigureAwait(false);
            }
            else
            {
                await _next
                    .Invoke(context)
                    .ConfigureAwait(false);
            }
        }
    }
}
