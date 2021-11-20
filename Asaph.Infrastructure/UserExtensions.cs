using System;
using System.Collections.Generic;
using System.Linq;
using Asaph.Core.Domain.UserAggregate;

namespace Asaph.Infrastructure
{
    internal static class UserExtensions
    {
        /// <summary>
        /// Converts an Asaph user to a Microsoft Graph user.
        /// </summary>
        /// <param name="user">The user to convert.</param>
        /// <param name="domainName">Azure AD B2C domain name.</param>
        /// <param name="extensionsAppClientId">Extensions app client id.</param>
        /// <param name="roles">Roles.</param>
        /// <returns>The conversion result.</returns>
        public static Microsoft.Graph.User ToMicrosoftGraphUser(
            this User user,
            string domainName,
            string extensionsAppClientId,
            IDictionary<string, string>? roles)
        {
            string fullName = user.FullName;

            string mailNickname = fullName.ToLower().Replace(' ', '.');

            // Create a user object for the user to add
            Microsoft.Graph.User microsoftGraphUser = new()
            {
                AccountEnabled = true,
                DisplayName = fullName,
                Mail = user.EmailAddress,
                MailNickname = mailNickname,
                MobilePhone = user.PhoneNumber,
                PasswordProfile = new()
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = Guid.NewGuid().ToString()
                },
                UserPrincipalName = $"{mailNickname}@{domainName}"
            };

            // Add roles for the user if any were provided
            if (roles != null)
            {
                string rolesString = string.Join(
                    ";", roles.Select(kv => $"{kv.Key}:{kv.Value}"));

                microsoftGraphUser.AdditionalData = new Dictionary<string, object>
                {
                    [extensionsAppClientId] = rolesString
                };
            }

            return microsoftGraphUser;
        }
    }
}
