namespace Asaph.Core.UseCases;

/// <summary>
/// Interface for use case boundaries.
/// </summary>
/// <typeparam name="TResponse">Response type.</typeparam>
/// <typeparam name="TOutput">Output type.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S2326:Unused type parameters should be removed",
    Justification = "Interface represents a contract.")]
public interface IBoundary<TResponse, TOutput>
{
}
