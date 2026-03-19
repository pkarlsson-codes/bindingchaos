using BindingChaos.SharedKernel.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Infrastructure.ExceptionHandling;

/// <summary>
/// Handles <see cref="AggregateNotFoundException"/> and writes a 404 Not Found ProblemDetails response.
/// </summary>
internal sealed class AggregateNotFoundExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not AggregateNotFoundException notFoundException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Resource not found",
                Detail = notFoundException.Message,
                Status = StatusCodes.Status404NotFound,
            },
        }).ConfigureAwait(false);
    }
}
