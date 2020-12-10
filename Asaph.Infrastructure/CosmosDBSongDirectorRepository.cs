using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace Asaph.Infrastructure
{
    public class CosmosDBSongDirectorRepository : CosmosDBRepository, IAsyncSongDirectorRepository
    {
        public CosmosDBSongDirectorRepository(CosmosDBConfiguration configuration) 
            : base(configuration with { ContainerId = nameof(SongDirector) }) { }

        public async Task<Guid> AddAsync(SongDirector songDirector)
        {
            // Create a new GUID for the new song director's id
            var id = Guid.NewGuid();

            // Create a song director document
            SongDirectorDocument songDirectorDocument = new()
            {
                Id = id.ToString(),
                EmailAddress = songDirector.EmailAddress,
                FullName = songDirector.FullName,
                PhoneNumber = songDirector.PhoneNumber,
                Rank = songDirector.Rank?.Name
            };

            // Get a song director container
            Container songDirectors = GetContainer();

            // Add the song director
            await songDirectors.CreateItemAsync(songDirectorDocument, new PartitionKey(songDirectorDocument.Id));

            // Set and return the id
            songDirector.Id = id;
            return id;
        }

        public Task<Rank?> FindRankAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }
    }
}

