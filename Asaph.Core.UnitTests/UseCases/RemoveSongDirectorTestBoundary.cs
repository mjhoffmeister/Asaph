using Asaph.Core.UseCases.RemoveSongDirector;

namespace Asaph.Core.UnitTests.UseCases;

/// <summary>
/// Test boundary implementation for the Remove Song Director use case.
/// </summary>
public class RemoveSongDirectorTestBoundary
    : IRemoveSongDirectorBoundary<RemoveSongDirectorResponse>
{
    /// <inheritdoc/>
    public RemoveSongDirectorResponse CannotRemoveSelf(RemoveSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public RemoveSongDirectorResponse InsufficientPermissions(RemoveSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public RemoveSongDirectorResponse RemovalFailed(RemoveSongDirectorResponse response)
    {
        return response;
    }

    /// <inheritdoc/>
    public RemoveSongDirectorResponse SongDirectorRemoved(RemoveSongDirectorResponse response)
    {
        return response;
    }
}
