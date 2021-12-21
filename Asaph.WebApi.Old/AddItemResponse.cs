using Hydra.NET;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Asaph.WebApi
{
    /// <summary>
    /// Add item response.
    /// </summary>
    public class AddItemResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemResponse"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="id">Id.</param>
        /// <param name="operations">Operations.</param>
        public AddItemResponse(
            Context context, string id, params Operation[] operations)
        {
            Context = context;
            Id = id;
            Operations = operations;
        }

        /// <summary>
        /// Context.
        /// </summary>
        [JsonPropertyName("@context")]
        public Context Context { get; }

        /// <summary>
        /// Id.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; }

        /// <summary>
        /// Operations.
        /// </summary>
        [JsonPropertyName("operation")]
        public IEnumerable<Operation>? Operations { get; }
    }
}
