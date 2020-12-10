using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.AddSongDirector
{
    public class AddSongDirectorInteractor<TOutput> : IAsyncUseCaseInteractor<AddSongDirectorRequest, TOutput>
    {
        private readonly IAddSongDirectorBoundary<TOutput> _boundary;
        private readonly IAsyncSongDirectorRepository _songDirectorRepository;

        public AddSongDirectorInteractor(
            IAsyncSongDirectorRepository songDirectorRepository,
            IAddSongDirectorBoundary<TOutput> boundary)
        {
            _songDirectorRepository = songDirectorRepository;
            _boundary = boundary;
        }

        public async Task<TOutput> HandleAsync(AddSongDirectorRequest request)
        {
            // Get the requester's rank
            Rank? requesterRank = await _songDirectorRepository.FindRankAsync(request.RequesterUsername);

            // Ensure that the requester is a grandmaster
            if (requesterRank != Rank.Grandmaster)
                return _boundary.InsufficientPermissions(AddSongDirectorResponse.InsufficientPermissions());

            // Validate the new song director
            Result<SongDirector> songDirectorCreateResult = SongDirector.TryCreate(
                request.FullName, request.EmailAddress, request.PhoneNumber, request.RankName);

            // Validation failure
            if (songDirectorCreateResult.IsFailed)
            {
                return _boundary.ValidationFailure(
                    AddSongDirectorResponse.ValidationFailure(
                        songDirectorCreateResult.Errors.Select(e => e.Message)));
            }

            // Add the song director
            Guid addedSongDirectorId = await _songDirectorRepository.AddAsync(songDirectorCreateResult.Value);

            // Return a response indicating that the song director was added
            return _boundary.SongDirectorAdded(
                AddSongDirectorResponse.SongDirectorAdded(addedSongDirectorId, request.FullName!));
        }
    }
}
