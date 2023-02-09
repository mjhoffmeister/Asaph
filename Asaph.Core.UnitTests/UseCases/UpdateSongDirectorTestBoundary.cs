using Asaph.Core.UseCases.UpdateSongDirector;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test implementation of the update song director boundary.
/// </summary>
internal class UpdateSongDirectorTestBoundary
    : IUpdateSongDirectorBoundary<UpdateSongDirectorResponse>
{
    /// <inheritdoc/>
    public UpdateSongDirectorResponse InsufficientPermissions(UpdateSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public UpdateSongDirectorResponse InvalidRequest(UpdateSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public UpdateSongDirectorResponse RequesterRankNotFound(UpdateSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public UpdateSongDirectorResponse SongDirectorUpdated(UpdateSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public UpdateSongDirectorResponse SongDirectorUpdateFailed(UpdateSongDirectorResponse response)
    {
        return response;
    }
}
