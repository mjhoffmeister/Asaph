using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Asaph.Infrastructure
{
    public record DynamoDBConfiguration(
        string AwsAccessKeyId,
        string AwsSecretAccessKey,
        string AwsRegionSystemName,
        string DynamoDBLocalUrl,
        bool UseDynamoDBLocal);

    /// <summary>
    /// Implementation of <see cref="IAsyncSongDirectorRepository"/> using AWS DynamoDB.
    /// </summary>
    public class DynamoDBSongDirectorRepository : IAsyncSongDirectorRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDBClient;
        private static readonly string _songDirectorTableName = "SongDirectors";

        private DynamoDBSongDirectorRepository(AmazonDynamoDBClient dynamoDBClient) =>
            _dynamoDBClient = dynamoDBClient;
            
        /// <summary>
        /// Tries to create a new <see cref="DynamoDBSongDirectorRepository"/>.
        /// </summary>
        /// <param name="configuration">DynamoDB configuration.</param>
        /// <returns>The result of the creation attempt.</returns>
        public async static Task<Result<DynamoDBSongDirectorRepository>> TryCreate(
            DynamoDBConfiguration configuration)
        {
            AmazonDynamoDBClient dynamoDBClient = new(
                configuration.AwsAccessKeyId,
                configuration.AwsSecretAccessKey,
                GetAmazonDynamoDBConfig(configuration));

            Result songDirectorsTableCreateResult = await CreateSongDirectorsTableIfNotExists(
                dynamoDBClient).ConfigureAwait(false);

            if (songDirectorsTableCreateResult.IsFailed)
                return songDirectorsTableCreateResult;

            return Result
                .Ok(new DynamoDBSongDirectorRepository(dynamoDBClient))
                .WithSuccess(songDirectorsTableCreateResult.Successes[0]);
        }

        public async Task<Result> TryAddAsync(SongDirector songDirector)
        {
            // Create a song director document
            SongDirectorDocument songDirectorDocument = new()
            {
                IsActive = songDirector.IsActive,
                EmailAddress = songDirector.EmailAddress,
                FullName = songDirector.FullName,
                PhoneNumber = songDirector.PhoneNumber ?? "",
                Rank = songDirector.Rank?.Name ?? ""
            };

            // Try to add the song director to DynamoDB
            try
            {
                // Send the request
                PutItemResponse addSongDirectorResponse = await _dynamoDBClient.PutItemAsync(
                    _songDirectorTableName,
                    songDirectorDocument.ToAttributeValueDictionary());

                // If the response doesn't indicate a success, return a failure result
                if (addSongDirectorResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    return Result.Fail(
                        $"HTTP status code: {addSongDirectorResponse.HttpStatusCode}.");
                }

                // Send an Ok result if the add succeeded
                return Result.Ok();
            }
            catch (InternalServerErrorException ex)
            {
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<Rank?>> TryFindRankAsync(string emailAddress)
        {
            try
            {
                // Create a key for the search
                Dictionary<string, AttributeValue> key = new()
                { 
                    { "EmailAddress", new AttributeValue(emailAddress) } 
                };

                // Get the song director with the provided email address
                // TODO: would QueryAsync be better?
                GetItemResponse getRankResponse = await _dynamoDBClient.GetItemAsync(
                    _songDirectorTableName, key);

                // If the song director wasn't found, return a failure result
                if (!getRankResponse.IsItemSet)
                {
                    return Result.Fail(
                        $"Could not find a song director with email address {emailAddress}.");
                }

                // Reference the song director's rank
                string rank = getRankResponse.Item[nameof(SongDirectorDocument.Rank)].S;

                // Return the conversion result
                return Rank.TryGetByName(rank);
            }
            catch (InternalServerErrorException ex)
            {
                return Result.Fail(ex.ToString());
            }

        }

        public async Task<Result<IEnumerable<SongDirector>>> TryGetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        private static async Task<Result> CreateSongDirectorsTableIfNotExists(
            AmazonDynamoDBClient dynamoDBClient)
        {
            ListTablesResponse listTablesResponse = await dynamoDBClient
                .ListTablesAsync()
                .ConfigureAwait(false);

            if (listTablesResponse.HttpStatusCode != HttpStatusCode.OK)
                return ListDynamoDBTablesFailedResult(listTablesResponse);

            if (!listTablesResponse.TableNames.Contains(_songDirectorTableName))
            {
                // Try to create the song directors table
                CreateTableResponse createTableResponse = await dynamoDBClient
                    .CreateTableAsync(GetCreateSongDirectorTableRequest())
                    .ConfigureAwait(false);

                if (createTableResponse.HttpStatusCode != HttpStatusCode.Created)
                    return CreateSongDirectorsTableFailedResult(createTableResponse);

                return Result.Ok().WithSuccess($"Created table {_songDirectorTableName}.");
            }

            return Result.Ok().WithSuccess($"The {_songDirectorTableName} table already exists.");
        }

        /// <summary>
        /// Converts a <see cref="DynamoDBConfiguration"/> object to a 
        /// <see cref="AmazonDynamoDBConfig"/> object.
        /// </summary>
        private static AmazonDynamoDBConfig GetAmazonDynamoDBConfig(
            DynamoDBConfiguration configuration)
        {
            // Configure the Dynamo DB client to use the local instance
            if (configuration.UseDynamoDBLocal)
            {
                return new()
                {
                    ServiceURL = configuration.DynamoDBLocalUrl
                };
            }

            // Amazon-hosted instance configuration

            RegionEndpoint regionEndpoint = RegionEndpoint
                .GetBySystemName(configuration.AwsRegionSystemName);

            return new()
            {
                RegionEndpoint = regionEndpoint
            };
        }

        private static CreateTableRequest GetCreateSongDirectorTableRequest() => new()
            {
                TableName = _songDirectorTableName,
                AttributeDefinitions = new()
                {
                    new AttributeDefinition("EmailAddress", ScalarAttributeType.S),
                },
                KeySchema = new()
                {
                    new KeySchemaElement("EmailAddress", KeyType.HASH)
                },
                ProvisionedThroughput = new()
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }
            };

        /// <summary>
        /// Gets a result indicating that the song directors table couldn't be created.
        /// </summary>
        /// <param name="createTableResponse">The result of the create table request</param>
        /// <returns><see cref="Result"/></returns>
        private static Result CreateSongDirectorsTableFailedResult(
            CreateTableResponse createTableResponse) => Result.Fail(
                $"Could not create the {_songDirectorTableName} table. HTTP status code: " +
                $"{createTableResponse.HttpStatusCode}");

        /// <summary>
        /// Gets a result indicating that DynamoDB tables couldn't be listed.
        /// </summary>
        /// <param name="listTablesResponse">The result of the list tables request.</param>
        /// <returns><see cref="Result"/></returns>
        private static Result ListDynamoDBTablesFailedResult(
            ListTablesResponse listTablesResponse) => Result.Fail(
                "Could not list DynamoDB tables, so the existance of the " +
                $"{_songDirectorTableName} table could not be determined. HTTP status code: " +
                $"{listTablesResponse.HttpStatusCode}");

        public Task<Result<IEnumerable<Rank>>> TryGetRanks()
        {
            throw new System.NotImplementedException();
        }
    }
}
