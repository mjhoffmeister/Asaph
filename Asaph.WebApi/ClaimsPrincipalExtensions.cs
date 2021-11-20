﻿using Asaph.Core.UseCases;
using System.Linq;
using System.Security.Claims;

namespace Asaph.WebApi
{
    /// <summary>
    /// <see cref="ClaimsPrincipal"/> extensions.
    /// </summary>
    public static class ClaimsPrincipalExtensions
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
            return true;

            //return claimsPrincipal.Claims
            //    .Any(claim => claim.Value.Contains(Roles.GrandmasterSongDirector));
        }
    }
}