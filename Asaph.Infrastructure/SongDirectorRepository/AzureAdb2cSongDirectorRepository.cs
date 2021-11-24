using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.UseCases;
using Azure.Identity;
using FluentResults;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asaph.Infrastructure.SongDirectorRepository;

/// <summary>
/// Azure AD B2C implementation of <see cref="ISongDirectorRepositoryFragment"/>.
/// </summary>
[SuppressMessage(
    "Minor Code Smell",
    "S101:Types should be named in PascalCase",
    Justification = "Analyzer doesn't correctly see AD B2C as a single acronym in PascalCase.")]
public class AzureAdb2cSongDirectorRepository : ISongDirectorRepositoryFragment
{
    // Azure AD B2C domain name
    private readonly string _domain;

    // The property name of the Roles extension
    private readonly string _rolesPropertyName;

    // Song director job title
    private readonly string _songDirectorJobTitle = "Song Director";

    // Select string for a user. Includes on properties needed for song directors.
    private readonly string _userSelectString;

    // Graph service client
    private readonly GraphServiceClient _graphServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAdb2cSongDirectorRepository"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public AzureAdb2cSongDirectorRepository(AzureAdb2cConfiguration configuration)
    {
        _domain = configuration.Domain!;

        _rolesPropertyName = $"extension_{configuration.ExtensionsAppClientId}_Roles";

        _userSelectString = $"displayName,id,mail,mobilePhone,{_rolesPropertyName}";

        ClientSecretCredential clientSecretCredential = new(
            configuration.TenantId, configuration.ClientId, configuration.ClientSecret);

        _graphServiceClient = new GraphServiceClient(clientSecretCredential);
    }

    /// <summary>
    /// Gets the operation execution for the method.
    /// </summary>
    /// <param name="methodName">Method name.</param>
    /// <returns>The operation execution order, or 0 if order doesn't matter.</returns>
    public int GetOperationExecutionOrder(string methodName) => methodName switch
    {
        nameof(ISongDirectorRepositoryFragment.TryAddAsync) => 1,
        _ => 0,
    };

    /// <inheritdoc/>
    public async Task<Result<SongDirectorDataModel>> TryAddAsync(
        SongDirectorDataModel songDirectorDataModel)
    {
        // Create a user object for the user to add
        User userToAdd = GetUserFromDataModel(songDirectorDataModel);

        try
        {
            // Add the user to Azure AD B2C
            User addedUser = await _graphServiceClient
                .Users
                .Request()
                .Select("id")
                .AddAsync(userToAdd)
                .ConfigureAwait(false);

            // Update user id on the song director data model
            songDirectorDataModel.Id = addedUser.Id;

            // Return a success result with the new user id
            return Result.Ok(songDirectorDataModel);
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Tries to find a property of a song director by song director id.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="id">Id.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>The result of the attempt.</returns>
    public async Task<Result<TProperty>> TryFindPropertyByIdAsync<TProperty>(
        string id, string propertyName)
    {
        string? selectString = GetSelectString(propertyName);

        if (selectString == null)
            return Result.Fail($"{propertyName} is not stored in Azure AD B2C.");

        User? searchResult = await _graphServiceClient
            .Users[id]
            .Request()
            .Select(selectString)
            .GetAsync()
            .ConfigureAwait(false);

        if (searchResult == null)
            return Result.Fail($"Could not find user with id {id} in Azure AD B2C.");

        return TryGetValue<TProperty>(searchResult, propertyName);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<SongDirectorDataModel>>> TryGetAllAsync()
    {
        try
        {
            IEnumerable<User> users = await _graphServiceClient
                .Users
                .Request()
                .Filter($"jobTitle eq '{_songDirectorJobTitle}'")
                .Select(_userSelectString)
                .GetAsync()
                .ConfigureAwait(false);

            IEnumerable<SongDirectorDataModel> songDirectorDataModels = users
                .Select(user => new SongDirectorDataModel(
                    user.Id,
                    user.DisplayName,
                    user.Mail,
                    user.MobilePhone,
                    GetUserSongDirectorRankName(user),
                    isActive: null));

            return Result.Ok(songDirectorDataModels);
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<SongDirectorDataModel>> TryGetByIdAsync(string id)
    {
        try
        {
            // Get the user from Azure AD B2C
            User user = await _graphServiceClient
                .Users[id]
                .Request()
                .Select(_userSelectString)
                .GetAsync()
                .ConfigureAwait(false);

            // The user wasn't found
            if (user == null)
                return Result.Fail($"Could not find a user with id {id} in Azure AD B2C.");

            // The user was found
            return Result.Ok(new SongDirectorDataModel(
                user.Id,
                user.DisplayName,
                user.Mail,
                user.MobilePhone,
                GetUserSongDirectorRankName(user),
                isActive: null));
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Tries to remove a song director by id.
    /// </summary>
    /// <param name="id">The id of the song director to remove.</param>
    /// <returns>The result of the attempt.</returns>
    public async Task<Result> TryRemoveByIdAsync(string id)
    {
        try
        {
            await _graphServiceClient
                .Users[id]
                .Request()
                .DeleteAsync()
                .ConfigureAwait(false);

            return Result.Ok();
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result> TryRollBackRemove(SongDirectorDataModel songDirectorDataModel)
    {
        try
        {
            string? songDirectorId = songDirectorDataModel.Id;

            if (songDirectorId == null)
            {
                return Result.Fail(
                    "Cannot restore a song director in Azure AD B2C without an id.");
            }

            await _graphServiceClient
                .Directory
                .DeletedItems[songDirectorId]
                .Restore()
                .Request()
                .PostAsync()
                .ConfigureAwait(false);

            return Result.Ok();
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result> TryUpdateAsync(SongDirectorDataModel songDirectorDataModel)
    {
        try
        {
            string? songDirectorId = songDirectorDataModel.Id;

            if (songDirectorId == null)
            {
                return Result.Fail(
                    "Cannot update a song director in Azure AD B2C without an id.");
            }

            await _graphServiceClient
                .Users[songDirectorId]
                .Request()
                .UpdateAsync(GetUserFromDataModel(songDirectorDataModel))
                .ConfigureAwait(false);

            return Result.Ok();
        }
        catch (ClientException ex)
        {
            // Return an failure result if a ClientException was thrown
            return Result.Fail(ex.Message);
        }
        catch (ServiceException ex)
        {
            // Return an failure result if a ServiceException was thrown
            return Result.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Gets a select string for a property.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>
    /// The select string for the property, or null if the property isn't stored in Azure AD
    /// B2C.
    /// </returns>
    private string? GetSelectString(string propertyName) =>
        propertyName switch
        {
            nameof(SongDirectorDataModel.EmailAddress) => "mail",
            nameof(SongDirectorDataModel.FullName) => "displayName",
            nameof(SongDirectorDataModel.PhoneNumber) => "mobilePhone",
            nameof(SongDirectorDataModel.RankName) => _rolesPropertyName,
            _ => null,
        };

    /// <summary>
    /// Gets a user from a data model.
    /// </summary>
    /// <param name="songDirectorDataModel">The data model to convert.</param>
    /// <returns><see cref="User"/>.</returns>
    private User GetUserFromDataModel(SongDirectorDataModel songDirectorDataModel)
    {
        string? fullName = songDirectorDataModel.FullName;

        string? mailNickname = fullName?.ToLower()?.Replace(' ', '.');

        // Create a user object for the user to add
        User microsoftGraphUser = new()
        {
            AccountEnabled = true,
            DisplayName = fullName,
            JobTitle = _songDirectorJobTitle,
            Mail = songDirectorDataModel.EmailAddress,
            MailNickname = mailNickname,
            MobilePhone = songDirectorDataModel.PhoneNumber,
            PasswordProfile = new()
            {
                ForceChangePasswordNextSignIn = false,
                Password = Guid.NewGuid().ToString(),
            },
            UserPrincipalName = $"{mailNickname}@{_domain}",
        };

        // Add the song director rank role, if the song director has a rank
        if (songDirectorDataModel.RankName is string rankName)
        {
            string rolesString = $"{Roles.SongDirectorRank}:{rankName}";

            microsoftGraphUser.AdditionalData = new Dictionary<string, object>
            {
                [_rolesPropertyName] = rolesString,
            };
        }

        return microsoftGraphUser;
    }

    /// <summary>
    /// Tries to get a value from an Azure AD B2C user.
    /// </summary>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="user">User.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>The result of the attempt.</returns>
    private Result<TProperty> TryGetValue<TProperty>(User user, string propertyName)
    {
        // Try to get rank
        if (propertyName == nameof(SongDirectorDataModel.RankName))
        {
            string? rankName = GetUserSongDirectorRankName(user);

            Result<Rank?> getRankResult = Rank.TryGetByName(rankName);

            if (getRankResult is Result<TProperty> getRankPropertyResult)
                return getRankPropertyResult;
        }

        // Try to get email address, full name, or phone number
        Result<string?> stringResult = propertyName switch
        {
            nameof(SongDirectorDataModel.EmailAddress) => Result.Ok<string?>(user.Mail),
            nameof(SongDirectorDataModel.FullName) => Result.Ok<string?>(user.DisplayName),
            nameof(SongDirectorDataModel.PhoneNumber) => Result.Ok(
                string.IsNullOrWhiteSpace(user.MobilePhone) ? null : user.MobilePhone),
            _ => Result.Fail("Invalid Azure AD B2C property."),
        };

        if (stringResult.IsSuccess && stringResult is Result<TProperty> getStringPropertyResult)
            return getStringPropertyResult;

        return Result.Fail($"Could not find {propertyName} property for a song director" +
                $"with id {user.Id} in Azure AD B2C.");
    }

    /// <summary>
    /// Gets the song director rank name for a user.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns>Song director rank name, if present; null, otherwise.</returns>
    private string? GetUserSongDirectorRankName(User user)
    {
        if (user.AdditionalData?.TryGetValue(
            _rolesPropertyName, out object? rolesStringObject) == true
            && rolesStringObject is JsonElement rolesJsonElement)
        {
            string? rolesString = rolesJsonElement.GetString();

            if (rolesString != null)
            {
                Match roleMatch = Regex.Match(rolesString, $"{Roles.SongDirectorRank}:(.+);?");

                if (roleMatch.Success)
                    return roleMatch.Groups[1].Value;
            }
        }

        return null;
    }
}