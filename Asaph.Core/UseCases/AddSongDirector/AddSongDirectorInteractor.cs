using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.AddSongDirector
{
    public class AddSongDirectorInteractor<TOutput> 
        : IAsyncUseCaseInteractor<AddSongDirectorRequest, TOutput>
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
            // Reference the requester's username
            string requesterUsername = request.RequesterUsername;

            // Get the requester's rank
            Result<Rank?> getRequesterRankResult = await
                _songDirectorRepository.TryFindRankAsync(requesterUsername);
            Rank? requesterRank = getRequesterRankResult.Value;

            // Couldn't get the requester's rank
            if (getRequesterRankResult.IsFailed || requesterRank == null)
            {
                return _boundary.RequesterRankNotFound(
                    AddSongDirectorResponse.RequesterRankNotFound(
                        requesterUsername, getRequesterRankResult.GetErrorMessagesString()));
            }

            // Ensure that the requester is a grandmaster
            if (requesterRank != Rank.Grandmaster)
            {
                return _boundary.InsufficientPermissions(
                    AddSongDirectorResponse.InsufficientPermissions());
            }

            // Validate the new song director
            Result<SongDirector> songDirectorCreateResult = SongDirector.TryCreate(
                request.FullName,
                request.EmailAddress,
                request.PhoneNumber,
                request.RankName,
                request.IsActive);

            // Validation failure
            if (songDirectorCreateResult.IsFailed)
            {
                return _boundary.ValidationFailure(
                    AddSongDirectorResponse.ValidationFailure(
                        songDirectorCreateResult.Errors.Select(e => e.Message)));
            }

            // Add the song director
            Result<Guid> addSongDirectorResult = await _songDirectorRepository.TryAddAsync(
                songDirectorCreateResult.Value);

            if (addSongDirectorResult.IsFailed)
            {
                return _boundary.SongDirectorAddFailed(
                    AddSongDirectorResponse.SongDirectorAddFailed(
                        addSongDirectorResult.GetErrorMessagesString()));
            }

            // Return a response indicating that the song director was added
            return _boundary.SongDirectorAdded(
                AddSongDirectorResponse.SongDirectorAdded(
                    addSongDirectorResult.Value, request.FullName!));
        }
    }
}
