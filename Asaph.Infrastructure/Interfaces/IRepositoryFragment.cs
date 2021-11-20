namespace Asaph.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for a part of a repository. Used for cases where an entity's data is stored across
    /// multiple sources.
    /// </summary>
    public interface IRepositoryFragment
    {
        /// <summary>
        /// Gets the execution order for an operation. Used for orchestrating operations where the
        /// order of execution across repository fragments matter.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <returns>The operation execution order.</returns>
        int GetOperationExecutionOrder(string methodName);
    }
}
