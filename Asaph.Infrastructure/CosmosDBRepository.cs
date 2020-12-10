using Microsoft.Azure.Cosmos;
using System;

namespace Asaph.Infrastructure
{
    public abstract class CosmosDBRepository : IDisposable
    {
        private readonly CosmosDBConfiguration _configuration;

        private Container? _container;

        private CosmosClient? _cosmosClient;

        protected CosmosDBRepository(CosmosDBConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// Gets a Cosmos DB container for the respository.
        /// </summary>
        /// <returns>The container.</returns>
        protected Container GetContainer()
        {
            if (_container is null)
            {
                if (_cosmosClient is null)
                    _cosmosClient = new(_configuration.ConnectionString);

                Database database = _cosmosClient.GetDatabase(_configuration.DatabaseId);
                _container = database.GetContainer(_configuration.ContainerId);
            }

            return _container;
        }

        #region IDisposable implementation

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _cosmosClient?.Dispose();

                _cosmosClient = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
