using BindingChaos.SharedKernel.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Infrastructure.ExceptionHandling;

/// <summary>
/// Handles <see cref="DomainException"/> (including <see cref="BusinessRuleViolationException"/>
/// and <see cref="InvariantViolationException"/>) and writes a 422 Unprocessable Entity ProblemDetails response.
/// </summary>
internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Business rule violation",
                Detail = domainException.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
            },
        }).ConfigureAwait(false);
    }
}
