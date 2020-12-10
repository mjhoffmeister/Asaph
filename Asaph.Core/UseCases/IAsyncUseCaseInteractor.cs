using System.Threading.Tasks;

namespace Asaph.Core.UseCases
{
    public interface IAsyncUseCaseInteractor<TRequest, TOutput>
    {
        public Task<TOutput> HandleAsync(TRequest request);
    }
}
