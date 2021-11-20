using System.Threading.Tasks;

namespace Asaph.Core.UseCases
{
    /// <summary>
    /// Interface for an async use case interactor.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IAsyncUseCaseInteractor<TRequest, TOutput>
    {
        /// <summary>
        /// Handles a use case request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The output of the request.</returns>
        public Task<TOutput> HandleAsync(TRequest request);
    }
}
