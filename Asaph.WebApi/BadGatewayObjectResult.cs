/// <summary>
/// Bad Gateway object result.
/// </summary>
internal class BadGatewayObjectResult : IResult
{
    public BadGatewayObjectResult(object value, string contentType) => 
        (ContentType, Value) = (contentType, value);

    /// <summary>
    /// Gets the value for the <c>Content-Type</c> header.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// The object result.
    /// </summary>
    public object Value { get; }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        int badGatewayStatusCode = StatusCodes.Status502BadGateway;

        Type valueType = Value.GetType();

        LogBadGatewayObjectResultExecuting(httpContext, valueType);

        httpContext.Response.StatusCode = badGatewayStatusCode;

        return httpContext.Response.WriteAsJsonAsync(
            Value, Value.GetType(), options: null, contentType: ContentType);
    }

    /// <summary>
    /// Logs that a <see cref="BadGatewayObjectResult"/> is executing.
    /// </summary>
    /// <param name="httpContext">HTTP context.</param>
    /// <param name="valueType">Value type of the object result.</param>
    private void LogBadGatewayObjectResultExecuting(HttpContext httpContext, Type valueType)
    {
        ILoggerFactory? loggerFactory = httpContext
            .RequestServices
            .GetRequiredService<ILoggerFactory>();

        ILogger? logger = loggerFactory.CreateLogger(GetType());

        logger?.LogInformation(
            $"Executing {nameof(BadGatewayObjectResult)}. Writing value of type {valueType.Name}.");
    }
}
