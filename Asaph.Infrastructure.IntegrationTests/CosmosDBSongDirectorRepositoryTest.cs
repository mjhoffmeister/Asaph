using Asaph.Core.Domain.SongDirectorAggregate;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Asaph.Infrastructure.IntegrationTests
{
    public class CosmosDBSongDirectorRepositoryTest : IClassFixture<CosmosDbFixture>
    {
        private readonly CosmosDbFixture _cosmosDbFixture;

        public CosmosDBSongDirectorRepositoryTest(CosmosDbFixture cosmosDbFixture) =>
            _cosmosDbFixture = cosmosDbFixture;

        public string ConnectionString { get; }

        [Theory]
        [InlineData("Asaph", "SongDirector", "/id")]
        public async Task Add_ValidSongDirector_ReturnsGuid(string databaseId, string containerId, string partitionKeyPath)
        {
            // Arrange

            // Initialize a test DB
            await _cosmosDbFixture.InitializeDatabase(databaseId, containerId, partitionKeyPath);

            // Create a song director
            var songDirector = SongDirector
                .TryCreate("John Doe", "john.doe@gmail.com", "124-456-3456", "Apprentice")
                .Value;


            // Create a song director repository
            CosmosDBSongDirectorRepository cosmosDBSongDirectorRepository = new(
                new(_cosmosDbFixture.ConnectionString, containerId, databaseId));

            // Act

            Guid newSongDirectorId = await cosmosDBSongDirectorRepository.AddAsync(songDirector);

            // Assert

            Assert.NotEqual(Guid.Empty, newSongDirectorId);
        }
    }

    public class CosmosDbFixture : IDisposable
    {
        private CosmosClient _cosmosClient;
        private Database _database;

        public string ConnectionString { get; private set; }

        public CosmosClient CosmosClient { get; }

        /// <summary>
        /// Initializes a database for the test.
        /// </summary>
        public async Task InitializeDatabase(string databaseId, string containerId, string partitionKeyPath)
        {
            ConnectionString = new ConfigurationBuilder()
                .AddUserSecrets<CosmosDBSongDirectorRepositoryTest>()
                .Build()
                .GetConnectionString("cosmosDBPrimaryConnectionString");

            _cosmosClient = new(ConnectionString);

            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId, 400);

            _ = await _database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);
        }

        #region IDisposable and IAsyncDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cosmosClient != null)
                {
                    _ = _database.DeleteAsync().Result;
                    _cosmosClient.Dispose();
                    _cosmosClient = null;
                }
            }
        }

        #endregion
    }
}
