using Asaph.Core.Domain.SongDirectorAggregate;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Infrastructure.IntegrationTests
{
    /// <summary>
    /// To start DynamoDB locally, navigate to the directory where the local version was extracted,
    /// and run the following command:
    /// java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb
    /// Source: https://amzn.to/3zjA7B0
    /// </summary>
    public class DynamoDBSongDirectorRepositoryTest
    {
        public DynamoDBSongDirectorRepositoryTest() => InitializeAwsCredentials();

        private string? AwsAccessKeyId { get; set; }

        private string? AwsSecretAccessKey { get; set; }

        private static string LocalDynamoDBUrl => "http://localhost:8000";

        [Theory]
        [InlineData("us-east-2", true)]
        public async Task TryAddAsync_ValidSongDirector_ReturnsSuccessResult(
            string awsRegionSystemName, bool useDynamoDBLocal)
        {
            // Arrange

            // Create a song director
            var songDirector = SongDirector
                .TryCreate("John Doe", "john.doe@gmail.com", "124-456-3456", "Apprentice", true)
                .Value;

            // Create a song director repository
            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = await
                GetDynamoDBSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

            // Act

            Result addSongDirectorResult = await dynamoDBSongDirectorRepository
                .TryAddAsync(songDirector)
                .ConfigureAwait(false);

            // Assert

            Assert.True(addSongDirectorResult.IsSuccess);
        }

        [Theory]
        [InlineData("john.doe@gmail.com", "us-east-2", true)]
        public async Task TryFindRank_SongDirectorExistsWithRank_ReturnsRank(
            string emailAddress, string awsRegionSystemName, bool useDynamoDBLocal)
        {
            // Arrange

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = await
                GetDynamoDBSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

            // Act

            Result<Rank?> findRankResult = await dynamoDBSongDirectorRepository
                .TryFindRankAsync(emailAddress)
                .ConfigureAwait(false);

            // Assert

            Assert.NotNull(findRankResult.Value);
        }

        [Theory]
        [InlineData("us-east-2", true, 1)]
        public async Task TryGetAll_SongDirectorsExist_ReturnsExpectedCount(
            string awsRegionSystemName, bool useDynamoDBLocal, int expectedCount)
        {
            // Arrange

            DynamoDBSongDirectorRepository dynamoDBSongDirectorRepository = await
                GetDynamoDBSongDirectorRepository(awsRegionSystemName, useDynamoDBLocal);

            // Act

            Result<IEnumerable<SongDirector>> getAllSongDirectorsResult = await
                dynamoDBSongDirectorRepository
                    .TryGetAllAsync()
                    .ConfigureAwait(false);

            // Assert

            Assert.Equal(expectedCount, getAllSongDirectorsResult.Value.Count());
        }

        private async Task<DynamoDBSongDirectorRepository> GetDynamoDBSongDirectorRepository(
            string awsRegionSystemName, bool useDynamoDBLocal)
        {
            DynamoDBConfiguration dynamoDBConfiguration = new(
                AwsAccessKeyId,
                AwsSecretAccessKey,
                awsRegionSystemName,
                LocalDynamoDBUrl,
                useDynamoDBLocal);

            Result<DynamoDBSongDirectorRepository> createDynamoDBSongDirectorRepository = await
                DynamoDBSongDirectorRepository
                    .TryCreate(dynamoDBConfiguration)
                    .ConfigureAwait(false);

            return createDynamoDBSongDirectorRepository.Value;
        }

        private void InitializeAwsCredentials()
        {
            IConfiguration userSecretsConfiguration = new ConfigurationBuilder()
                    .AddUserSecrets<DynamoDBSongDirectorRepositoryTest>()
                    .Build();

            AwsAccessKeyId = userSecretsConfiguration["awsAccessKeyId"];
            AwsSecretAccessKey = userSecretsConfiguration["awsSecretAccessKey"];
        }
    }
}
