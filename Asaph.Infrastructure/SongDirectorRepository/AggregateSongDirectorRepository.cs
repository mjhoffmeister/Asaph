using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.Infrastructure.SongDirectorRepository;

public record AddToFragmentResult(
    ISongDirectorRepositoryFragment Fragment, Result<SongDirectorDataModel> Result);

public record AddToFragmentsResult(
    IEnumerable<Result<SongDirectorDataModel>> Failures,
    IEnumerable<AddToFragmentResult> Successes);

/// <summary>
/// Handles data consistency for song directors stored across multiple data sources.
/// </summary>
public class AggregateSongDirectorRepository : IAsyncRepository<SongDirector>
{
    private readonly IEnumerable<ISongDirectorRepositoryFragment> _repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateSongDirectorRepository"/> class.
    /// </summary>
    /// <param name="repositories">Repository fragments.</param>
    public AggregateSongDirectorRepository(
        IEnumerable<ISongDirectorRepositoryFragment> repositories) =>
            _repositories = repositories;

    /// <inheritdoc/>
    public async Task<Result<SongDirector>> TryAddAsync(SongDirector songDirector)
    {
        // Convert the song director to a data model
        SongDirectorDataModel songDirectorDataModel = new(songDirector);

        // Add the song director to each repository fragment
        AddToFragmentsResult addToFragmentsResult = await
            TryAddToFragments(songDirectorDataModel).ConfigureAwait(false);

        // If there were failures, roll back any successful adds
        if (addToFragmentsResult.Failures.Any())
            return await HandleAddFailures(addToFragmentsResult).ConfigureAwait(false);

        // Reference the new song director's id
        string? newSongDirectorId = addToFragmentsResult
            .Successes
            .FirstOrDefault(result => result.Result.Value.Id != null)?
            .Result.Value.Id;

        // Return a failure result if no failures occurred but a new id wasn't generated
        if (newSongDirectorId == null)
        {
            return Result.Fail($"No errors occurred while adding {songDirector.FullName}, but no " +
                $"id was returned. Manual remediation is required.");
        }

        // Update the song director's id
        songDirector.UpdateId(newSongDirectorId);

        // If all adds succeeded, return the success result
        return Result.Ok(songDirector);
    }

    /// <inheritdoc/>
    public async Task<Result<TProperty>> TryFindPropertyByIdAsync<TProperty>(
        string id, string propertyName)
    {
        // Normalize rank property name
        if (propertyName == "Rank")
            propertyName = "RankName";

        // Initiate the find tasks across repository fragments
        IEnumerable<Task<Result<TProperty>>> findPropertyByIdTasks = _repositories
            .Select(r => r.TryFindPropertyByIdAsync<TProperty>(id, propertyName));

        // Wait for the tasks to complete
        IEnumerable<Result<TProperty>> findPropertyByIdResults = await Task
            .WhenAll(findPropertyByIdTasks)
            .ConfigureAwait(false);

        // If all successful results contain the same value, return the first successful result
        if (findPropertyByIdResults.Where(r => r.IsSuccess).Distinct().Any())
            return findPropertyByIdResults.First(r => r.IsSuccess);

        // If no results are successes, return a failure result
        if (findPropertyByIdResults.All(r => r.IsFailed))
        {
            return Result
                .Fail($"Could not find the requested song director property.")
                .WithErrors(findPropertyByIdResults.SelectMany(r => r.Errors));
        }

        // If there are multiple successful results that contain different value return a
        // failure result
        return Result.Fail($"The song director with id {id} has inconsistent values for " +
            $"{propertyName} across data sources. Manual remediation is required.");
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<SongDirector>>> TryGetAllAsync()
    {
        // Get song directors from all fragments
        IEnumerable<Result<IEnumerable<SongDirectorDataModel>>> getAllResults = await Task
            .WhenAll(_repositories
                .Select(r => r.TryGetAllAsync()))
            .ConfigureAwait(false);

        // Return a failure result if any fragments encountered errors
        if (getAllResults.Any(result => result.IsFailed))
        {
            return Result
                .Fail("Could not get song directors.")
                .WithErrors(getAllResults
                    .Where(result => result.IsFailed)
                    .SelectMany(result => result.Errors));
        }

        // Try to merge data models
        IEnumerable<Result<SongDirector>> mergeResults = getAllResults
            .SelectMany(repo => repo.Value)
            .GroupBy(dataModel => dataModel.Id)
            .Select(group => TryGetSongDirectorFromDataModels(group.Key, group));

        // Return a failure result if all merges failed
        if (mergeResults.All(result => result.IsFailed))
        {
            return Result.Fail(
                "Song director data sources are out of sync. Manual remediation is required.");
        }

        // Select any merges that failed
        IEnumerable<Result<SongDirector>> failedMergeResults = mergeResults
            .Where(result => result.IsFailed);

        // Create a success result with successful merges
        Result<IEnumerable<SongDirector>> result = Result.Ok(
            mergeResults
                .Where(result => result.IsSuccess)
                .Select(result => result.Value));

        // If the merges partially succeeded, add the errors for the failed merges.
        if (failedMergeResults.Any())
            return result.WithErrors(failedMergeResults.SelectMany(result => result.Errors));

        // Return the result
        return result;
    }

    /// <inheritdoc/>
    public async Task<Result<SongDirector>> TryGetByIdAsync(string id)
    {
        IEnumerable<Task<Result<SongDirectorDataModel>>> getByIdTasks = _repositories
            .Select(r => r.TryGetByIdAsync(id));

        IEnumerable<Result<SongDirectorDataModel>> getByIdResults = await Task
            .WhenAll(getByIdTasks)
            .ConfigureAwait(false);

        IEnumerable<Result<SongDirectorDataModel>> failedResults = getByIdResults
            .Where(r => r.IsFailed);

        if (failedResults.Any())
        {
            return Result
                .Fail($"Could not get song director with id {id}.")
                .WithErrors(failedResults.SelectMany(r => r.Errors));
        }

        return TryGetSongDirectorFromDataModels(id, getByIdResults.Select(r => r.Value));
    }

    /// <inheritdoc/>
    public async Task<Result> TryRemoveByIdAsync(string id)
    {
        Result tryRemoveWithRollBackOnFailure = await TryOperationWithRollBackSupportAsync(
            id,
            repo => repo.TryRemoveByIdAsync(id),
            (repo, originalSongDirector) => repo.TryRollBackRemove(originalSongDirector))
        .ConfigureAwait(false);

        if (tryRemoveWithRollBackOnFailure.IsFailed)
            return Result.Fail($"Failed to remove the song director with id {id}.");

        return tryRemoveWithRollBackOnFailure;
    }

    /// <inheritdoc/>
    public async Task<Result> TryUpdateAsync(SongDirector songDirector)
    {
        // Reference the song director id
        string? id = songDirector.Id;

        // Return a failure response if id isn't set
        if (id == null)
            return Result.Fail("Cannot update a song director without an id.");

        Result tryUpdateWithRollBackOnFailure = await TryOperationWithRollBackSupportAsync(
            id,
            repo => repo.TryUpdateAsync(new SongDirectorDataModel(songDirector)),
            (repo, originalSongDirector) => repo.TryUpdateAsync(originalSongDirector))
        .ConfigureAwait(false);

        if (tryUpdateWithRollBackOnFailure.IsFailed)
            return Result.Fail($"Failed to update the song director with id {id}.");

        return tryUpdateWithRollBackOnFailure;
    }

    /// <summary>
    /// Handles add failures.
    /// </summary>
    /// <param name="addToFragmentsResult">The result of the add operations.</param>
    /// <returns>The result of the add failure handler.</returns>
    private static async Task<Result<SongDirector>> HandleAddFailures(
        AddToFragmentsResult addToFragmentsResult)
    {
        // Deconstruct the result
        (IEnumerable<Result<SongDirectorDataModel>> failures,
         IEnumerable<AddToFragmentResult> successes)
            = addToFragmentsResult;

        if (successes.Any())
        {
            IEnumerable<Result> rollBackResults = await TryRollBackAdds(successes)
                .ConfigureAwait(false);

            return Result.Merge(
                failures.Merge(), rollBackResults.Merge()).ToResult<SongDirector>();
        }

        return failures.Merge().ToResult<SongDirector>();
    }

    /// <summary>
    /// Tries to get a song director from fragmented data models.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="songDirectorDataModels">Data models.</param>
    /// <returns>The result of the attempt.</returns>
    private static Result<SongDirector> TryGetSongDirectorFromDataModels(
        string? songDirectorId, IEnumerable<SongDirectorDataModel> songDirectorDataModels)
    {
        if (songDirectorId == null)
            return Result.Fail("Cannot merge song directos without a song director id.");

        Result<SongDirectorDataModel> mergeResult = SongDirectorDataModel
            .TryMerge(songDirectorDataModels);

        if (mergeResult.IsFailed)
        {
            return Result
                .Fail("There are data consistency issues for song director with id " +
                        $"{songDirectorId}. Manual remediation is required.")
                .WithErrors(mergeResult.Errors);
        }

        SongDirectorDataModel songDirectorDataModel = mergeResult.Value;

        Result<SongDirector> createSongDirectorResult = SongDirector.TryCreate(
            songDirectorDataModel.FullName,
            songDirectorDataModel.EmailAddress,
            songDirectorDataModel.PhoneNumber,
            songDirectorDataModel.RankName,
            songDirectorDataModel.IsActive);

        if (createSongDirectorResult.IsFailed)
        {
            return Result
                .Fail($"The data for song director with id {songDirectorId} is invalid. " +
                        "Manual remediation is required.")
                .WithErrors(createSongDirectorResult.Errors);
        }

        SongDirector songDirector = createSongDirectorResult.Value;
        songDirector.UpdateId(songDirectorId);
        return Result.Ok(songDirector);
    }

    /// <summary>
    /// Tries to roll back adds.
    /// </summary>
    /// <param name="addResults">Add results.</param>
    /// <returns>The result of the attempt.</returns>
    private static async Task<IEnumerable<Result>> TryRollBackAdds(
        IEnumerable<AddToFragmentResult> addResults)
    {
        return await Task
            .WhenAll(addResults
                .Select(r => r.Fragment.TryRemoveByIdAsync(r.Result.Value.Id!)))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to roll back operations.
    /// </summary>
    /// <param name="getSongDirectorTask">
    /// The task for getting the original song director.
    /// </param>
    /// <param name="successRepositories">
    /// Repositories for which the operation succeeded, therefore needed a roll back.
    /// </param>
    /// <param name="tryRollBackAsync">
    /// The roll back operation to try for each repository for which the operation succeeded.
    /// </param>
    /// <returns>The async operation.</returns>
    private static async Task<Result> TryRollBackOperationAsync(
        Task<Result<SongDirector>> getSongDirectorTask,
        IEnumerable<ISongDirectorRepositoryFragment> successRepositories,
        Func<ISongDirectorRepositoryFragment, SongDirectorDataModel, Task<Result>>
            tryRollBackAsync)
    {
        Result<SongDirector> getSongDirectorResult = getSongDirectorTask.Result;

        if (getSongDirectorResult.IsFailed)
        {
            return Result
                .Fail(
                    "An error occurred during part of the operation. The " +
                    "successful parts of the operation could not be rolled back " +
                    "because the song director the operation affected could not " +
                    "be cached.");
        }

        IEnumerable<Result> rollBackResult = await Task.WhenAll(successRepositories
            .Select(repo => tryRollBackAsync(
                repo, new(getSongDirectorResult.Value))))
            .ConfigureAwait(false);

        if (rollBackResult.Any(result => result.IsFailed))
        {
            return Result
                .Fail(
                    "An error occurred during part of the operation, and failures" +
                    "were encountered during the roll back attempts.")
                .WithErrors(rollBackResult
                    .Where(result => result.IsFailed)
                    .SelectMany(result => result.Errors));
        }

        return Result.Fail(
            "An error occurred during part of the operation. All successful parts" +
            "were rolled back.");
    }

    /// <summary>
    /// Tries to add a song director to each repository fragment.
    /// </summary>
    /// <param name="songDirectorDataModel">The song director to add.</param>
    /// <returns>The result of the attempt.</returns>
    private async Task<AddToFragmentsResult> TryAddToFragments(
        SongDirectorDataModel songDirectorDataModel)
    {
        // Sort repositories for the operation
        IOrderedEnumerable<ISongDirectorRepositoryFragment> orderByTryAddAsync =
            _repositories.OrderBy(r => r.GetOperationExecutionOrder(nameof(TryAddAsync)));

        // Create lists for failed and successful adds
        List<Result<SongDirectorDataModel>> failures = new();
        List<AddToFragmentResult> successes = new();

        foreach (ISongDirectorRepositoryFragment repository in orderByTryAddAsync)
        {
            // Try to add the data model to the fracment
            Result<SongDirectorDataModel> addResult = await repository
                .TryAddAsync(songDirectorDataModel)
                .ConfigureAwait(false);

            // Add the result to the appropriate list base on failure or success
            if (addResult.IsSuccess)
            {
                successes.Add(new(repository, addResult));
            }
            else
            {
                failures.Add(addResult);
                break;
            }
        }

        return new(failures, successes);
    }

    /// <summary>
    /// Tries to execute an operation with roll back support.
    /// </summary>
    /// <param name="songDirectorId">Song director id.</param>
    /// <param name="tryOperationAsync">The operation to try.</param>
    /// <param name="tryRollBackAsync">
    /// The roll back operation to execute if the operation fails.
    /// </param>
    /// <returns>The async operation.</returns>
    private async Task<Result> TryOperationWithRollBackSupportAsync(
        string songDirectorId,
        Func<ISongDirectorRepositoryFragment, Task<Result>> tryOperationAsync,
        Func<ISongDirectorRepositoryFragment, SongDirectorDataModel, Task<Result>>
            tryRollBackAsync)
    {
        // Cache the song director so that it can be restored if any deletes fail
        Task<Result<SongDirector>> getSongDirectorTask = TryGetByIdAsync(songDirectorId);

        // Wait for the caching to complete before trying any operations on that song director
        getSongDirectorTask.Wait();

        // Keep a reference of repositories for which operations were successful in case roll
        // backs are needed
        List<ISongDirectorRepositoryFragment> successRepositories = new();

        // Execute the operation for each repository fragment
        foreach (ISongDirectorRepositoryFragment repository in _repositories)
        {
            // Try to execute the operation
            Result operationResult = await tryOperationAsync(repository)
                .ConfigureAwait(false);

            // If the operation failed, roll back any successful operations and return a failure
            // result
            if (operationResult.IsFailed)
            {
                if (successRepositories.Any())
                {
                    Result rollBackResult = await TryRollBackOperationAsync(
                        getSongDirectorTask, successRepositories, tryRollBackAsync)
                        .ConfigureAwait(false);

                    return rollBackResult.WithErrors(operationResult.Errors);
                }

                return operationResult;
            }
            else
            {
                // If it succeeded, add the repository to the successful repository list
                successRepositories.Add(repository);
            }
        }

        // If all removes succeeded, return a success result
        return Result.Ok();
    }
}