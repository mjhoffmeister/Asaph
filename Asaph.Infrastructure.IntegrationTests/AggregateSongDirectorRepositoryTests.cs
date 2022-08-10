using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Infrastructure.SongDirectorRepository;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Infrastructure.IntegrationTests;

/// <summary>
/// Tests the <see cref="AggregateSongDirectorRepository"/> class.
/// </summary>
/// <remarks>
/// To start DynamoDB locally, navigate to the directory where the local version was extracted,
/// and run the following command:
/// <c>java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb</c>
/// Source: https://amzn.to/3zjA7B0.
/// </remarks>
[SuppressMessage(
    "Major Code Smell",
    "S1118:Utility classes should not have public constructors",
    Justification = "A non-static class is needed for getting user secrets by type.")]
public class AggregateSongDirectorRepositoryTests
{
    /// <summary>
    /// Tests adding a song director.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="rankName">Rank name.</param>
    /// <param name="isActive">Active indicator.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData(
        "us-east-2",
        true,
        "{song director name}",
        "{song director email address}",
        "{song director phone number}",
        "{song director rank}",
        true)
    ]
    public static async Task TryAddAsync_ValidSongDirector_Succeeds(
        string awsRegionSystemName,
        bool useDynamoDBLocal,
        string fullName,
        string emailAddress,
        string phoneNumber,
        string rankName,
        bool isActive)
    {
        // Arrange

        SongDirector songDirector = SongDirector
            .TryCreate(fullName, emailAddress, phoneNumber, rankName, isActive)
            .Value;

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

        // Act

        Result<SongDirector> addResult = await aggregateSongDirectorRepository
            .TryAddAsync(songDirector)
            .ConfigureAwait(false);

        // Assert

        Assert.True(addResult.IsSuccess);

        Console.WriteLine(addResult.Value.Id);
    }

    /// <summary>
    /// Tests getting rank for a song director.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether or not to use Dynamo DB local.</param>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="expectedRankName">Expected rank name.</param>
    /// <param name="userSecretsSection">User secrets section.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData("us-east-2", false, "{user id}", "{rank name}", "{user secrets section}")]
    public static async Task TryFindPropertyById_Rank_ReturnsExpectedRankName(
        string awsRegionSystemName,
        bool useDynamoDBLocal,
        string songDirectorId,
        string expectedRankName,
        string userSecretsSection)
    {
        // Arrange

        string propertyName = "Rank";

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(
                awsRegionSystemName, useDynamoDBLocal, userSecretsSection);

        // Act

        Result<Rank?> getRankResult = await aggregateSongDirectorRepository
            .TryFindPropertyByIdAsync<Rank?>(songDirectorId, propertyName)
            .ConfigureAwait(false);

        // Assert

        Assert.Equal(expectedRankName, getRankResult.Value?.Name);
    }

    /// <summary>
    /// Tests getting all song directors.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="expectedSongDirectorCount">Expected song director count.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData("us-east-2", false, 1)]
    public static async Task TryGetAllAsync(
        string awsRegionSystemName,
        bool useDynamoDBLocal,
        int expectedSongDirectorCount)
    {
        // Arrange

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

        // Act

        Result<IEnumerable<SongDirector>> getAllResult = await aggregateSongDirectorRepository
            .TryGetAllAsync()
            .ConfigureAwait(false);

        // Assert

        Assert.Equal(expectedSongDirectorCount, getAllResult.Value.Count());
    }

    /// <summary>
    /// Tests getting a song director by id.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="userSecretsSection">User secrets section.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData("us-east-2", false, "{user id}", "{user secrets section}")]
    public static async Task TryGetByIdAsync_ExistingSongDirector_Succeeds(
        string awsRegionSystemName,
        bool useDynamoDBLocal,
        string songDirectorId,
        string userSecretsSection)
    {
        // Arrange

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(
                awsRegionSystemName, useDynamoDBLocal, userSecretsSection);

        // Act

        Result<SongDirector> getByIdResult = await aggregateSongDirectorRepository
            .TryGetByIdAsync(songDirectorId)
            .ConfigureAwait(false);

        // Assert

        Assert.True(getByIdResult.IsSuccess);
    }

    /// <summary>
    /// Tests removing a song director by id.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="songDirectorId">Song director id.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData("us-east-2", true, "{user id}")]
    public static async Task TryRemoveByIdAsync_ExistingSongDirector_Succeeds(
        string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId)
    {
        // Arrange

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

        // Act

        Result removeByIdResult = await aggregateSongDirectorRepository
            .TryRemoveByIdAsync(songDirectorId)
            .ConfigureAwait(false);

        // Assert

        Assert.True(removeByIdResult.IsSuccess);
    }

    /// <summary>
    /// Tests updating a song director.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="rankName">Rank name.</param>
    /// <param name="isActive">Active indicator.</param>
    /// <param name="userSecretsSection">User secrets section.</param>
    /// <returns>The async operation.</returns>
    [Theory]
    [InlineData(
        "us-east-2",
        false,
        "{song director id}",
        "{song director name}",
        "{song director email}",
        "{song director phone number}",
        "{song director rank}",
        true,
        "{user secrets section}")
    ]
    public static async Task TryUpdateAsync_ExistingSongDirector_Succeeds(
        string awsRegionSystemName,
        bool useDynamoDBLocal,
        string songDirectorId,
        string fullName,
        string emailAddress,
        string phoneNumber,
        string rankName,
        bool isActive,
        string userSecretsSection)
    {
        // Arrange

        SongDirector songDirector = SongDirector
            .TryCreate(fullName, emailAddress, phoneNumber, rankName, isActive)
            .Value;

        songDirector.UpdateId(songDirectorId);

        AggregateSongDirectorRepository aggregateSongDirectorRepository =
            GetAggregateSongDirectorRepository(
                awsRegionSystemName, useDynamoDBLocal, userSecretsSection);

        // Act

        Result updateResult = await aggregateSongDirectorRepository
            .TryUpdateAsync(songDirector)
            .ConfigureAwait(false);

        // Assert

        Assert.True(updateResult.IsSuccess);
    }

    /// <summary>
    /// Gets an <see cref="AggregateSongDirectorRepository"/> for the test.
    /// </summary>
    /// <param name="awsRegionSystemName">AWS region system name.</param>
    /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
    /// <param name="userSecretsSection">
    /// The configuration section to use in user secrets. Defaults to "Dev".
    /// </param>
    /// <returns><see cref="AggregateSongDirectorRepository"/>.</returns>
    private static AggregateSongDirectorRepository GetAggregateSongDirectorRepository(
        string awsRegionSystemName, bool useDynamoDBLocal, string? userSecretsSection = "Dev")
    {
        ConfigurationBuilder builder = new();

        builder.AddUserSecrets<AggregateSongDirectorRepositoryTests>();

        IConfiguration configuration = builder.Build();

        AzureAdb2cConfiguration azureAdb2CConfiguration = new(
            configuration[$"{userSecretsSection}:AzureAdb2c:ClientId"],
            configuration[$"{userSecretsSection}:AzureAdb2c:ClientSecret"],
            configuration[$"{userSecretsSection}:AzureAdb2c:Domain"],
            configuration[$"{userSecretsSection}:AzureAdb2c:ExtensionsAppClientId"],
            configuration[$"{userSecretsSection}:AzureAdb2c:TenantId"]);

        AzureAdb2cSongDirectorRepository azureAdb2CSongDirectorRepository = new(
            azureAdb2CConfiguration);

        DynamoDBConfiguration dynamoDBConfiguration = new(
            configuration[$"{userSecretsSection}:DynamoDB:AwsAccessKeyId"],
            configuration[$"{userSecretsSection}:DynamoDB:AwsSecretAccessKey"],
            awsRegionSystemName,
            configuration[$"{userSecretsSection}:DynamoDB:TableNamePrefix"],
            configuration[$"{userSecretsSection}:DynamoDB:DynamoDBLocalUrl"],
            useDynamoDBLocal);

        DynamoDBSongDirectorRepository amazonDynamoDBSongDirectorRepository = new(
            dynamoDBConfiguration);

        return new(
            new ISongDirectorRepositoryFragment[]
            {
                azureAdb2CSongDirectorRepository,
                amazonDynamoDBSongDirectorRepository,
            });
    }
}