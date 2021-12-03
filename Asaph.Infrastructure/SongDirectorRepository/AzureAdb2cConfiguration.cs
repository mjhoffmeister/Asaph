using System.Diagnostics.CodeAnalysis;

namespace Asaph.Infrastructure.SongDirectorRepository;

/// <summary>
/// Azure AD B2C configuration.
/// </summary>
[SuppressMessage(
    "Minor Code Smell",
    "S101:Types should be named in PascalCase",
    Justification = "Analyzer doesn't correctly see AD B2C as a single acronym in PascalCase.")]
public class AzureAdb2cConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAdb2cConfiguration"/> class.
    /// </summary>
    public AzureAdb2cConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAdb2cConfiguration"/> class.
    /// </summary>
    /// <param name="clientId">Client id.</param>
    /// <param name="clientSecret">Client secret.</param>
    /// <param name="domain">Domain.</param>
    /// <param name="extensionsAppClientId">Extensions app client id.</param>
    /// <param name="tenantId">Tenant id.</param>
    public AzureAdb2cConfiguration(
        string clientId,
        string clientSecret,
        string domain,
        string extensionsAppClientId,
        string tenantId)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Domain = domain;
        ExtensionsAppClientId = extensionsAppClientId;
        TenantId = tenantId;
    }

    /// <summary>
    /// Client id.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Domain.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Extensions app client id.
    /// </summary>
    public string? ExtensionsAppClientId { get; set; }

    /// <summary>
    /// Tenant id.
    /// </summary>
    public string? TenantId { get; set; }
}