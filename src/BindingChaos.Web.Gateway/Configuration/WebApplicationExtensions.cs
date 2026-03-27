using BindingChaos.Web.Gateway.Middleware;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Extension methods for configuring the WebApplication middleware pipeline.
/// </summary>
internal static partial class WebApplicationExtensions
{
    /// <summary>
    /// Configures the request pipeline for development environments.
    /// Includes OpenAPI, Swagger UI, and development-specific middleware.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseOpenApi();
        app.UseSwaggerUi();

        return app;
    }

    /// <summary>
    /// Configures security-related middleware including global error handling, CORS,
    /// security headers, CSRF protection, and authentication/authorization.
    /// Order is critical:
    /// 1. Global error handling (catches all exceptions)
    /// 2. CORS (must be early to handle preflight requests)
    /// 3. Security headers (apply to all responses)
    /// 4. Authentication (establish identity)
    /// 5. CSRF protection (validate requests from authenticated users)
    /// 6. Authorization (check permissions).
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureSecurityPipeline(this WebApplication app)
    {
        app.UseGlobalErrorHandling();
        app.UseCors();
        app.UseSecurityHeaders();
        app.UseAuthentication();
        app.UseCsrfProtection();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configures API-specific middleware and endpoint mapping.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureApiPipeline(this WebApplication app)
    {
        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// Validates that all required services and configuration are available.
    /// This helps catch configuration issues at startup rather than runtime.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required services or configuration are missing.</exception>
    public static WebApplication ValidateStartupRequirements(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

        try
        {
            var requiredServices = new[]
            {
                typeof(IConfiguration),
                typeof(IWebHostEnvironment),
            };

            foreach (var serviceType in requiredServices)
            {
                var service = scope.ServiceProvider.GetService(serviceType);
                if (service == null)
                {
                    throw new InvalidOperationException($"Required service {serviceType.Name} is not registered.");
                }
            }

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var requiredSections = new[]
            {
                "CorePlatform:BaseAddress",
                "Authentication:OIDC:Authority",
                "Authentication:OIDC:ClientId",
            };

            foreach (var section in requiredSections)
            {
                var value = configuration[section];
                if (string.IsNullOrWhiteSpace(value))
                {
                    Logs.LogConfigurationMissing(logger, section);
                }
            }

            Logs.LogStartupValidationCompleted(logger);
        }
        catch (Exception ex)
        {
            Logs.LogStartupValidationFailed(logger, ex);
            throw;
        }

        return app;
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Startup validation failed")]
        internal static partial void LogStartupValidationFailed(ILogger logger, Exception? exception);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Startup validation completed successfully")]
        internal static partial void LogStartupValidationCompleted(ILogger logger);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Configuration section '{Section}' is missing or empty")]
        internal static partial void LogConfigurationMissing(ILogger logger, string section);
    }
}