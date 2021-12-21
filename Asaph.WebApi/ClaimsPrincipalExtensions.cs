using Asaph.Core.UseCases;
using System.Security.Claims;

/// <summary>
/// <see cref="ClaimsPrincipal"/> extensions.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Determines whether a claims principal has the grandmaster song director role.
    /// </summary>
    /// <param name="claimsPrincipal">Claims principal.</param>
    /// <returns>
    /// True if the claims principal has the grandmaster song director role; false, otherwise.
    /// </returns>
    public static bool IsGrandmasterSongDirector(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims
            .Any(claim => claim.Value.Contains(Roles.GrandmasterSongDirector));
    }
}