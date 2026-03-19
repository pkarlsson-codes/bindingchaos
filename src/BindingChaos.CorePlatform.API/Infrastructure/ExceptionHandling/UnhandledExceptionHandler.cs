using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Infrastructure.ExceptionHandling;

/// <summary>
/// Fallback handler that catches all unhandled exceptions and writes a 500 Internal Server Error ProblemDetails response.
/// Must be registered last so that more specific handlers take priority.
/// </summary>
internal sealed class UnhandledExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<UnhandledExceptionHandler> logger) : IExceptionHandler
{
    private static readonly Action<ILogger, string, Exception> LogUnhandledException =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(LogUnhandledException)),
            "Unhandled exception for request {Path}");

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogUnhandledException(logger, httpContext.Request.Path, exception);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
            },
        }).ConfigureAwait(false);
    }
}
