using BindingChaos.SharedKernel.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Infrastructure.ExceptionHandling;

/// <summary>
/// Handles <see cref="ForbiddenException"/> and writes a 403 Forbidden ProblemDetails response.
/// </summary>
internal sealed class ForbiddenExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ForbiddenException forbiddenException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Forbidden",
                Detail = forbiddenException.Message,
                Status = StatusCodes.Status403Forbidden,
            },
        }).ConfigureAwait(false);
    }
}
