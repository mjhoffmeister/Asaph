using System;
using System.Linq;
using FluentResults;

namespace Asaph.Core
{
    public static class FluentResultsExtensions
    {
        /// <summary>
        /// Gets all error messages and combines them into a single string, separated by new lines.
        /// </summary>
        /// <param name="result">The result for which to get error messages.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetErrorMessagesString(this Result result) =>
            string.Join(Environment.NewLine, result.Errors.Select(e => e.Message));

        /// <summary>
        /// Gets all error messages and combines them into a single string, separated by new lines.
        /// </summary>
        /// <param name="result">The result for which to get error messages.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetErrorMessagesString<T>(this Result<T> result) =>
            string.Join(Environment.NewLine, result.Errors.Select(e => e.Message));
    }
}
