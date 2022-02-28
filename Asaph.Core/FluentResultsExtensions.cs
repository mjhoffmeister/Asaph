using FluentResults;
using System;
using System.Linq;

namespace Asaph.Core;

/// <summary>
/// Extentions for FluentResults.
/// </summary>
public static class FluentResultsExtensions
{
    /// <summary>
    /// Gets all error messages and combines them into a single string, separated by new lines.
    /// </summary>
    /// <param name="result">The result for which to get error messages.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string GetErrorMessagesString(this Result result) =>
        string.Join(Environment.NewLine, result.Errors.Select(e => e.Message));

    /// <summary>
    /// Gets all error messages and combines them into a single string, separated by new lines.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Result>.</typeparam>
    /// <param name="result">The result for which to get error messages.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string GetErrorMessagesString<T>(this Result<T> result) =>
        string.Join(Environment.NewLine, result.Errors.Select(e => e.Message));
}
