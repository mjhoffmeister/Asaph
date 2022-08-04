using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using FluentResults;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Asaph.Infrastructure.SongDirectorRepository;

public record struct DynamoDBConfiguration(
    string AwsAccessKeyId,
    string AwsSecretAccessKey,
    string AwsRegionSystemName,
    string TableNamePrefix,
    string DynamoDBLocalUrl,
    bool UseDynamoDBLocal);

/// <summary>
/// Amazon Dynamo DB implementation of <see cref="ISongDirectorRepositoryFragment"/>.
/// </summary>
public class DynamoDBSongDirectorRepository : ISongDirectorRepositoryFragment
{
    private readonly AmazonDynamoDBClient _dynamoDBClient;
    private readonly DynamoDBContextConfig _dynamoDBContextConfig;
    private readonly string _songDirectorTableName = "SongDirectors";

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBSongDirectorRepository"/>
    /// class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public DynamoDBSongDirectorRepository(DynamoDBConfiguration configuration)
    {
        _dynamoDBClient = new(
            configuration.AwsAccessKeyId,
            configuration.AwsSecretAccessKey,
            GetAmazonDynamoDBConfig(configuration));

        _dynamoDBContextConfig = GetDynamoDBContextConfig(configuration);

        if (!string.IsNullOrWhiteSpace(configuration.TableNamePrefix))
            _songDirectorTableName = configuration.TableNamePrefix + _songDirectorTableName;
    }

    /// <summary>
    /// Gets the operation execution for the method.
    /// </summary>
    /// <param name="methodName">Method name.</param>
    /// <returns>The operation execution order, or 0 if order doesn't matter.</returns>
    public int GetOperationExecutionOrder(string methodName) =>
        methodName switch
        {
            nameof(ISongDirectorRepositoryFragment.TryAddAsync) => 2,
            _ => 0,
        };

    /// <inheritdoc/>
    public async Task<Result<SongDirectorDataModel>> TryAddAsync(
        SongDirectorDataModel songDirectorDataModel)
    {
        // Return an error response if id isn't set
        if (songDirectorDataModel.Id == null)
            return Result.Fail("Id must be set before adding a song director to Dynamo DB.");

        // Create the song directors table if it doesn't exist
        Result songDirectorsTableCreateResult = await CreateSongDirectorsTableIfNotExists()
            .ConfigureAwait(false);

        // Return a failure result if the song directors table doesn't exist and couldn't be
        // created
        if (songDirectorsTableCreateResult.IsFailed)
            return songDirectorsTableCreateResult;

        // Try to add the song director to DynamoDB
        try
        {
            using DynamoDBContext context = GetContext();

            await context
                .SaveAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Send an Ok result if the add succeeded
            return Result.Ok(songDirectorDataModel);
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
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
        // Build the request for finding the property
        QueryRequest findPropertyRequest = new()
        {
            TableName = _songDirectorTableName,
            KeyConditionExpression = "Id = :v_Id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":v_Id"] = new AttributeValue(id),
            },
            ProjectionExpression = propertyName,
        };

        // Query the database
        QueryResponse? response = await _dynamoDBClient
            .QueryAsync(findPropertyRequest)
            .ConfigureAwait(false);

        // Return a failure response if the song director wasn't found
        if (response == null || response?.Items.Count == 0)
        {
            return Result.Fail($"Could not find {propertyName} property for a song director" +
                $"with id {id}. Song director not found.");
        }

        // Convert items to a dictionary to help with checking for the property
        Dictionary<string, AttributeValue> values = new(response!.Items.First());

        // Return a failure response if the property wasn't found
        if (!values.TryGetValue(propertyName, out AttributeValue? value))
        {
            return Result.Fail(
                $"Song director with id {id} doesn't have a {propertyName} property.");
        }

        // Return the value
        return value.TryGetValue<TProperty>();
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<SongDirectorDataModel>>> TryGetAllAsync()
    {
        try
        {
            ScanResponse scanResponse = await _dynamoDBClient
                .ScanAsync(new ScanRequest(_songDirectorTableName))
                .ConfigureAwait(false);

            if (scanResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                return Result.Fail(
                    "Could not get all song directors from Dynamo DB. HTTP status code: " +
                    $"{scanResponse.HttpStatusCode}.");
            }

            IEnumerable<SongDirectorDataModel> songDirectorDataModels = scanResponse
                .Items
                .Select(i => i.ToSongDirectorDataModel());

            return Result.Ok(songDirectorDataModels);
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<SongDirectorDataModel>> TryGetByIdAsync(string id)
    {
        try
        {
            using DynamoDBContext context = GetContext();

            SongDirectorDataModel? songDirectorDataModel = await context
                .LoadAsync<SongDirectorDataModel>(id)
                .ConfigureAwait(false);

            if (songDirectorDataModel == null)
            {
                return Result.Fail(
                    $"Could not find a song director with id {id} in Dynamo DB.");
            }

            return Result.Ok(songDirectorDataModel);
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
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
            using DynamoDBContext context = GetContext();

            await context
                .DeleteAsync<SongDirectorDataModel>(id)
                .ConfigureAwait(false);

            return Result.Ok();
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result> TryRollBackRemove(SongDirectorDataModel songDirectorDataModel)
    {
        try
        {
            Result<SongDirectorDataModel> tryAddSongDirector = await TryAddAsync(
                songDirectorDataModel).ConfigureAwait(false);

            return tryAddSongDirector.ToResult();
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result> TryUpdateAsync(SongDirectorDataModel songDirectorDataModel)
    {
        try
        {
            using DynamoDBContext context = GetContext();

            await context
                .SaveAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            return Result.Ok();
        }
        catch (AmazonDynamoDBException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (AmazonServiceException ex)
        {
            return Result.Fail(ex.Message);
        }
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
                ServiceURL = configuration.DynamoDBLocalUrl,
            };
        }

        // Amazon-hosted instance configuration
        RegionEndpoint regionEndpoint = RegionEndpoint
            .GetBySystemName(configuration.AwsRegionSystemName);

        return new()
        {
            RegionEndpoint = regionEndpoint,
        };
    }

    /// <summary>
    /// Gets context configuration.
    /// </summary>
    /// <param name="configuration"><see cref="DynamoDBConfiguration"/>.</param>
    /// <returns><see cref="DynamoDBContextConfig"/>.</returns>
    private static DynamoDBContextConfig GetDynamoDBContextConfig(
        DynamoDBConfiguration configuration)
    {
        return new()
        {
            TableNamePrefix = configuration.TableNamePrefix,
        };
    }

    /// <summary>
    /// Gets a result indicating that the song directors table couldn't be created.
    /// </summary>
    /// <param name="createTableResponse">The result of the create table request.</param>
    /// <returns><see cref="Result"/>The result.</returns>
    private Result CreateSongDirectorsTableFailedResult(
        CreateTableResponse createTableResponse)
    {
        return Result.Fail(
            $"Could not create the {_songDirectorTableName} table. HTTP status code: " +
            $"{createTableResponse.HttpStatusCode}");
    }

    /// <summary>
    /// Gets a context for a DynamoDB operation.
    /// </summary>
    /// <returns><see cref="DynamoDBContext"/>.</returns>
    private DynamoDBContext GetContext()
    {
        return new DynamoDBContext(_dynamoDBClient, _dynamoDBContextConfig);
    }

    /// <summary>
    /// Gets a request for creating the song directors table.
    /// </summary>
    /// <returns><see cref="CreateTableRequest"/>.</returns>
    private CreateTableRequest GetCreateSongDirectorsTableRequest() => new()
    {
        TableName = _songDirectorTableName,
        AttributeDefinitions = new()
        {
            new AttributeDefinition("Id", ScalarAttributeType.S),
        },
        KeySchema = new()
        {
            new KeySchemaElement("Id", KeyType.HASH),
        },
        ProvisionedThroughput = new()
        {
            ReadCapacityUnits = 2,
            WriteCapacityUnits = 1,
        },
    };

    /// <summary>
    /// Gets a result indicating that DynamoDB tables couldn't be listed.
    /// </summary>
    /// <param name="listTablesResponse">The result of the list tables request.</param>
    /// <returns><see cref="Result"/>.</returns>
    private Result ListDynamoDBTablesFailedResult(
        ListTablesResponse listTablesResponse) => Result.Fail(
            "Could not list DynamoDB tables, so the existance of the " +
            $"{_songDirectorTableName} table could not be determined. HTTP status code: " +
            $"{listTablesResponse.HttpStatusCode}");

    private async Task<Result> CreateSongDirectorsTableIfNotExists()
    {
        using DynamoDBContext dynamoDBContext = GetContext();

        ListTablesResponse listTablesResponse = await _dynamoDBClient
            .ListTablesAsync()
            .ConfigureAwait(false);

        if (listTablesResponse.HttpStatusCode != HttpStatusCode.OK)
            return ListDynamoDBTablesFailedResult(listTablesResponse);

        if (!listTablesResponse.TableNames.Contains(_songDirectorTableName))
        {
            // Try to create the song directors table
            CreateTableResponse createTableResponse = await _dynamoDBClient
                .CreateTableAsync(GetCreateSongDirectorsTableRequest())
                .ConfigureAwait(false);

            if (createTableResponse.HttpStatusCode != HttpStatusCode.Created)
                return CreateSongDirectorsTableFailedResult(createTableResponse);

            return Result.Ok().WithSuccess($"Created table {_songDirectorTableName}.");
        }

        return Result.Ok().WithSuccess($"The {_songDirectorTableName} table already exists.");
    }
}
