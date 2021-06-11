using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Asaph.Infrastructure
{
    public class CosmosDBSongDirectorRepository : CosmosDBRepository, IAsyncSongDirectorRepository
    {
        public CosmosDBSongDirectorRepository(CosmosDBConfiguration configuration) 
            : base(configuration with { ContainerId = nameof(SongDirector) }) { }

        public async Task<Result> TryAddAsync(SongDirector songDirector)
        {
            // Create a song director document
            SongDirectorDocument songDirectorDocument = new()
            {
                IsActive = songDirector.IsActive,
                EmailAddress = songDirector.EmailAddress,
                FullName = songDirector.FullName,
                PhoneNumber = songDirector.PhoneNumber,
                Rank = songDirector.Rank?.Name
            };

            // Get a song director container
            Container songDirectors = GetContainer();

            // Add the song director
            await songDirectors.CreateItemAsync(songDirectorDocument);

            // Return a success result
            return Result.Ok();
        }

        public Task<Result<Rank?>> TryFindRankAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<SongDirector>>> TryGetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}

