using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Asaph.Core.Interfaces
{
    /// <summary>
    /// Generic asyncronous repository interface.
    /// </summary>
    /// <typeparam name="T">Type of the repository.</typeparam>
    public interface IAsyncRepository<T>
    {
        /// <summary>
        /// Tries to add the entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>
        /// A result of the attempt. On a success, the added entity is returned with its id set.
        /// </returns>
        Task<Result<T>> TryAddAsync(T entity);

        /// <summary>
        /// Tries to find the value of an entity's property by id.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="id">Id of the entity.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>The search result. On a success, the result value will be set.</returns>
        Task<Result<TProperty>> TryFindPropertyByIdAsync<TProperty>(
            string id, string propertyName);

        /// <summary>
        /// Tries to get all entities.
        /// </summary>
        /// <returns>The result of the attempt.</returns>
        Task<Result<IEnumerable<T>>> TryGetAllAsync();

        /// <summary>
        /// Tries to get an entity by id.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <returns>The result of the attempt.</returns>
        Task<Result<T>> TryGetByIdAsync(string id);

        /// <summary>
        /// Tries to remove an entity by id.
        /// </summary>
        /// <param name="id">The id of the entity to remove.</param>
        /// <returns>The result of the attempt.</returns>
        Task<Result> TryRemoveByIdAsync(string id);

        /// <summary>
        /// Tries to update an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The result of the attempt.</returns>
        Task<Result> TryUpdateAsync(T entity);
    }
}
