using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Infrastructure.SongDirectorRepository;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Infrastructure.IntegrationTests
{
    /// <summary>
    /// Tests the <see cref="DynamoDBSongDirectorRepository"/> class.
    /// </summary>
    /// <remarks>
    /// To start DynamoDB locally, navigate to the directory where the local version was extracted,
    /// and run the following command:
    /// java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb
    /// Source: https://amzn.to/3zjA7B0.
    /// </remarks>
    [SuppressMessage(
        "Major Code Smell",
        "S1118:Utility classes should not have public constructors",
        Justification = "A non-static class is needed for getting user secrets by type.")]
    public class DynamoDBSongDirectorRepositoryTests
    {
        /// <summary>
        /// Tests adding a song director to DynamoDB.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether or not to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director id to add.</param>
        /// <param name="isActive">Is active indicator to add.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "d7a068f8-461d-42f2-a561-5ea2f843c2b3", true)]
        public static async Task TryAddAsync_ValidSongDirector_Succeeds(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId, bool isActive)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                songDirectorId, null, null, null, null, isActive);

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result<SongDirectorDataModel> addResult = await dynamoDBSongDirectorRepository
                .TryAddAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.True(addResult.IsSuccess);
        }

        /// <summary>
        /// Tests getting rank for a song director.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether or not to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director id.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "5144f766-dae4-45f1-9f1a-dc52070e6910")]
        public static async Task TryFindPropertyByIdAsync_Rank_ReturnsFailedResult(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId)
        {
            // Arrange

            string propertyName = "RankName";

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result<Rank?> getRankResult = await dynamoDBSongDirectorRepository
                .TryFindPropertyByIdAsync<Rank?>(songDirectorId, propertyName)
                .ConfigureAwait(false);

            // Assert

            Assert.True(getRankResult.IsFailed);
        }

        /// <summary>
        /// Tests getting all song directors in Dynamo DB.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <param name="expectedSongDirectorDataModelCount">Expected song director count.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, 2)]
        public static async Task TryGetAllAsync(
            string awsRegionSystemName,
            bool useDynamoDBLocal,
            int expectedSongDirectorDataModelCount)
        {
            // Arrange

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result<IEnumerable<SongDirectorDataModel>> getAllResult = await
                dynamoDBSongDirectorRepository
                    .TryGetAllAsync()
                    .ConfigureAwait(false);

            // Assert

            Assert.Equal(expectedSongDirectorDataModelCount, getAllResult.Value.Count());
        }

        /// <summary>
        /// Tests getting a song director by id.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director id.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "7d89b90d-0df2-4483-b3d2-693be3c00a9d")]
        public static async Task TryGetByIdAsync_ExistingSongDirector_Succeeds(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId)
        {
            // Arrange

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result<SongDirectorDataModel> getByIdResult = await dynamoDBSongDirectorRepository
                .TryGetByIdAsync(songDirectorId)
                .ConfigureAwait(false);

            // Assert

            Assert.True(getByIdResult.IsSuccess);
        }

        /// <summary>
        /// Tests removing a song director from Dynamo DB.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director id.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "7d89b90d-0df2-4483-b3d2-693be3c00a9d")]
        public static async Task TryRemoveByIdAsync_ExistingUser_Succeeds(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId)
        {
            // Arrange

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result removeByIdResult = await dynamoDBSongDirectorRepository
                .TryRemoveByIdAsync(songDirectorId)
                .ConfigureAwait(false);

            // Assert

            Assert.True(removeByIdResult.IsSuccess);
        }

        /// <summary>
        /// Tests rolling back the removal of a song director.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director id.</param>
        /// <param name="isActive">Song director active indicator.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "d4774936-4e95-4935-8116-acf27f6ec93d", true)]
        public static async Task TryRollBackRemove_RemovedSongDirector_Succeeds(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId, bool isActive)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                songDirectorId, null, null, null, null, isActive);

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result rollBackRemoveResult = await dynamoDBSongDirectorRepository
                .TryRollBackRemove(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.True(rollBackRemoveResult.IsSuccess);
        }

        /// <summary>
        /// Tests updating a song director in Dynamo DB.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <param name="songDirectorId">Song director.</param>
        /// <param name="isActive">Active indicator for the song director.</param>
        /// <returns>The async operation.</returns>
        [Theory]
        [InlineData("us-east-2", true, "7d89b90d-0df2-4483-b3d2-693be3c00a9d", false)]
        public static async Task TryUpdateAsync_ExistingSongDirector_Succeeds(
            string awsRegionSystemName, bool useDynamoDBLocal, string songDirectorId, bool isActive)
        {
            // Arrange

            SongDirectorDataModel songDirectorDataModel = new(
                songDirectorId, null, null, null, null, isActive);

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = new(
                GetConfiguration(awsRegionSystemName, useDynamoDBLocal));

            // Act

            Result updateResult = await dynamoDBSongDirectorRepository
                .TryUpdateAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Assert

            Assert.True(updateResult.IsSuccess);
        }

        /// <summary>
        /// Gets configuration for Dynamo DB.
        /// </summary>
        /// <param name="awsRegionSystemName">AWS region system name.</param>
        /// <param name="useDynamoDBLocal">Indicates whether to use Dynamo DB local.</param>
        /// <returns><see cref="DynamoDBConfiguration"/>.</returns>
        private static DynamoDBConfiguration GetConfiguration(
            string awsRegionSystemName, bool useDynamoDBLocal)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<DynamoDBSongDirectorRepositoryTests>()
                .Build();

            string awsAccessKeyId = configuration["DynamoDB:AwsAccessKeyId"];
            string awsSecretAccessKey = configuration["DynamoDB:AwsSecretAccessKey"];
            string dynamoDBLocalUrl = configuration["DynamoDB:DynamoDBLocalUrl"];

            return new(
                awsAccessKeyId,
                awsSecretAccessKey,
                awsRegionSystemName,
                dynamoDBLocalUrl,
                useDynamoDBLocal);
        }
    }
}
