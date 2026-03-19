using BindingChaos.Infrastructure.API;
using Scalar.AspNetCore;

namespace BindingChaos.CorePlatform.API.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring the WebApplication pipeline.
/// </summary>
internal static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the request pipeline for development environments.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for method chaining.</returns>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseExceptionHandler();
        app.MapOpenApi();
        app.MapScalarApiReference();

        return app;
    }

    /// <summary>
    /// Configures the standard middleware pipeline.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for method chaining.</returns>
    public static WebApplication ConfigureStandardPipeline(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseCorrelationId();
        app.UseCors();

        // Remove HTTPS redirection for local development
        // app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configures endpoint mapping.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for method chaining.</returns>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapControllers();

        return app;
    }
}