using Asaph.Core.UseCases.GetSongDirectors;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test boundary implementation for the Get Song Directors use case.
/// </summary>
public class GetSongDirectorsTestBoundary
    : IGetSongDirectorsBoundary<GetSongDirectorsResponse>
{
    /// <inheritdoc/>
    public GetSongDirectorsResponse FailedToGetSongDirectors(GetSongDirectorsResponse response)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public GetSongDirectorsResponse InvalidRequesterEmailAddress(
        GetSongDirectorsResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public GetSongDirectorsResponse RequesterSongDirectorRankNotFound(
        GetSongDirectorsResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public GetSongDirectorsResponse Success(GetSongDirectorsResponse response)
    {
        return response;
    }
}
