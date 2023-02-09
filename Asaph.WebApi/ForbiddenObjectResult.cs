/// <summary>
/// Forbidden object result.
/// </summary>
public class ForbiddenObjectResult : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenObjectResult"/> class.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="contentType">Content type.</param>
    public ForbiddenObjectResult(object value, string contentType) =>
        (ContentType, Value) = (contentType, value);

    /// <summary>
    /// Gets the value for the <c>Content-Type</c> header.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// The object result.
    /// </summary>
    public object Value { get; }

    /// <inheritdoc/>
    public Task ExecuteAsync(HttpContext httpContext)
    {
        int forbiddenStatusCode = StatusCodes.Status403Forbidden;

        Type valueType = Value.GetType();

        LogForbiddenObjectResultExecuting(httpContext, valueType);

        httpContext.Response.StatusCode = forbiddenStatusCode;

        return httpContext.Response.WriteAsJsonAsync(
            Value, valueType, options: null, contentType: ContentType);
    }

    private void LogForbiddenObjectResultExecuting(HttpContext httpContext, Type valueType)
    {
        ILoggerFactory? loggerFactory = httpContext
            .RequestServices
            .GetRequiredService<ILoggerFactory>();

        ILogger? logger = loggerFactory.CreateLogger(GetType());

        logger?.LogInformation(
            "Executing {ResultName}. Writing value of type {ValueTypeName}.",
            nameof(ForbiddenObjectResult),
            valueType.Name);
    }
}