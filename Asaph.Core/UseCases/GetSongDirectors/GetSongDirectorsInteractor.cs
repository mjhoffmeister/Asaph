using Asaph.Core.Domain.PersonAggregate;
using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    public class GetSongDirectorsInteractor<TOutput> :
        IAsyncUseCaseInteractor<GetSongDirectorsRequest, TOutput>
    {
        private readonly IGetSongDirectorBoundary<TOutput> _boundary;
        private readonly IAsyncSongDirectorRepository _songDirectorRepository;

        public GetSongDirectorsInteractor(
            IAsyncSongDirectorRepository songDirectorRepository,
            IGetSongDirectorBoundary<TOutput> boundary)
        {
            _boundary = boundary;
            _songDirectorRepository = songDirectorRepository;
        }

        public async Task<TOutput> HandleAsync(GetSongDirectorsRequest request)
        {
            // Reference the requester's email address
            string? requesterEmailAddress = request.RequesterEmailAddress;

            // Validate that the requester's email address is valid
            if (!Person.ValidateEmailAddress(requesterEmailAddress).IsSuccess)
                return InvalidRequesterEmailAddressResponse();

            // TODO: validate that the requester is a person in Asaph

            // Try to find the requester's song director rank
            Result<Rank?> findRankResult = await _songDirectorRepository
                .TryFindRankAsync(requesterEmailAddress!);

            // Get song directors from the repository
            Result<IEnumerable<SongDirector>> getSongDirectorsResult = await _songDirectorRepository
                .TryGetAllAsync();

            // If the request failed, return a failure result
            if (getSongDirectorsResult.IsFailed)
                return FailedToGetSongDirectors(getSongDirectorsResult.GetErrorMessagesString());

            // Convert song director entities to models
            IEnumerable<SongDirectorModel> songDirectorModels = getSongDirectorsResult.Value!
                .Select(songDirector => 
                    songDirector.ConvertToModel(requesterEmailAddress, findRankResult.Value));

            // Return a success response
            return Success(songDirectorModels);
        }

        private TOutput FailedToGetSongDirectors(string errorMessage)
        {
            return _boundary.FailedToGetSongDirectors(
                GetSongDirectorsResponse.FailedToGetSongDirectors(errorMessage));
        }

        private TOutput InvalidRequesterEmailAddressResponse()
        {
            return _boundary.InvalidRequesterEmailAddress(
                GetSongDirectorsResponse.InvalidRequesterEmailAddress());
        }

        private TOutput Success(IEnumerable<SongDirectorModel> songDirectorModels) =>
            _boundary.Success(GetSongDirectorsResponse.Success(songDirectorModels));
    }
}
