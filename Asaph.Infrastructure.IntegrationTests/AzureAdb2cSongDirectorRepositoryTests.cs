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

namespace Asaph.Infrastructure.IntegrationTests
{
    /// <summary>
    /// Tests the <see cref="AzureAdb2cSongDirectorRepository"/> class.
    /// </summary>
    [SuppressMessage(
        "Major Code Smell",
        "S1118:Utility classes should not have public constructors",
        Justification = "A non-static class is needed for getting user secrets by type.")]
    public class AzureAdb2cSongDirectorRepositoryTests
    {
        /// <summary>
        /// Tests adding a song director to Azure AD B2C.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData(
            "John Doe",
            "john.doe@example.com",
            "123-456-1234",
            "Journeyer")
        ]
        public static async Task TryAddAsync_ValidSongDirector_ReturnsSongDirectorId(
            string fullName, string emailAddress, string? phoneNumber, string? rankName)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                null, fullName, emailAddress, phoneNumber, rankName, null);

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result<SongDirectorDataModel> addResult = await azureAdb2cUserRepository
                .TryAddAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.NotNull(addResult.Value);

            Console.WriteLine(addResult.Value.Id);
        }

        /// <summary>
        /// Tests finding a song director property in Azure AD B2C.
        /// </summary>
        /// <param name="songDirectorId">Song director id.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="expectedRankName">Expected rank name.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("{enter song director id here}", "RankName", "Grandmaster")]
        public static async Task TryFindPropertyById_Rank_ReturnsExpectedRankName(
            string songDirectorId, string propertyName, string expectedRankName)
        {
            // Arrange

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result<Rank?> getRankResult = await azureAdb2cUserRepository
                .TryFindPropertyByIdAsync<Rank?>(songDirectorId, propertyName)
                .ConfigureAwait(false);

            // Assert

            Assert.Equal(expectedRankName, getRankResult.Value?.Name);
        }

        /// <summary>
        /// Tests getting all song directors in Azure AD B2C.
        /// </summary>
        /// <param name="expectedSongDirectorDataModelCount">Expected song director count.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData(2)]
        public static async Task TryGetAllAsync_ExistingSongDirectors_ReturnsExpectedCount(
            int expectedSongDirectorDataModelCount)
        {
            // Arrange

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result<IEnumerable<SongDirectorDataModel>> getAllResult = await azureAdb2cUserRepository
                    .TryGetAllAsync()
                    .ConfigureAwait(false);

            // Assert

            Assert.Equal(expectedSongDirectorDataModelCount, getAllResult.Value.Count());
        }

        /// <summary>
        /// Tests getting a song director by id.
        /// </summary>
        /// <param name="songDirectorId">Song director id.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("d7a068f8-461d-42f2-a561-5ea2f843c2b3")]
        public static async Task TryGetByIdAsync_ExistingSongDirector_Succeeds(
            string songDirectorId)
        {
            // Arrange

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result<SongDirectorDataModel> getSongDirectorByIdResult = await azureAdb2cUserRepository
                .TryGetByIdAsync(songDirectorId)
                .ConfigureAwait(false);

            // Assert

            Assert.True(getSongDirectorByIdResult.IsSuccess);
        }

        /// <summary>
        /// Tests removing a song director from Azure AD B2C.
        /// </summary>
        /// <param name="songDirectorId">
        /// Song director id. Replace the song director id with the following template after
        /// removal: {enter song director id here}.
        /// </param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("{enter song director id here}")]
        public static async Task TryRemoveByIdAsync_ExistingSongDirector_Succeeds(
            string songDirectorId)
        {
            // Arrange

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result removeByIdResult = await azureAdb2cUserRepository
                .TryRemoveByIdAsync(songDirectorId)
                .ConfigureAwait(false);

            // Assert

            Assert.True(removeByIdResult.IsSuccess);
        }

        /// <summary>
        /// Tests rolling back the removal of a song director.
        /// </summary>
        /// /// <param name="songDirectorId">
        /// Song director id. Replace the song director id with the following template after
        /// removal: {enter song director id here}.
        /// </param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("{enter song director id here}")]
        public static async Task TryRollBackRemove_RemovedSongDirector_Succeeds(
            string songDirectorId)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                songDirectorId, null, null, null, null, null);

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result rollBackRemoveResult = await azureAdb2cUserRepository
                .TryRollBackRemove(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.True(rollBackRemoveResult.IsSuccess);
        }

        /// <summary>
        /// Tests updating a song director in Azure AD B2C.
        /// </summary>
        /// <param name="songDirectorId">Song director id.</param>
        /// <param name="fullName">Full name.</param>
        /// <param name="emailAddress">Email address.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="rankName">Rank name.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData(
            "7d89b90d-0df2-4483-b3d2-693be3c00a9d",
            "John Doe",
            "jdoe@example.com",
            "456-789-4567",
            "Master")]
        public static async Task TryUpdateAsync_ExistingSongDirector_Succeeds(
            string songDirectorId,
            string fullName,
            string emailAddress,
            string? phoneNumber,
            string? rankName)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                songDirectorId, fullName, emailAddress, phoneNumber, rankName, null);

            AzureAdb2cSongDirectorRepository azureAdb2cUserRepository = new(GetConfiguration());

            // Act

            Result updateResult = await azureAdb2cUserRepository
                .TryUpdateAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.True(updateResult.IsSuccess);
        }

        private static AzureAdb2cConfiguration GetConfiguration()
        {
            ConfigurationBuilder builder = new();

            builder.AddUserSecrets<AzureAdb2cSongDirectorRepositoryTests>();

            IConfiguration configuration = builder.Build();

            return new(
                configuration["AzureAdb2c:ClientId"],
                configuration["AzureAdb2c:ClientSecret"],
                configuration["AzureAdb2c:Domain"],
                configuration["AzureAdb2c:ExtensionsAppClientId"],
                configuration["AzureAdb2c:TenantId"]);
        }
    }
}
