namespace Asaph.Core.UseCases.GetSongDirectors;

/// <summary>
/// Default boundary implementation for the Get Song Directors use case.
/// </summary>
public class GetSongDirectorsDefaultBoundary
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