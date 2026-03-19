using System.Text.Json;
using BindingChaos.Infrastructure.API;

namespace BindingChaos.CorePlatform.API.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring WebApplicationBuilder services.
/// </summary>
internal static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures controllers with JSON options, filters, and model binders.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddConfiguredControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new BindingChaos.Infrastructure.Querying.SortDescriptorsModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
                new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(
                    new Microsoft.AspNetCore.Mvc.ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status422UnprocessableEntity,
                    });
        });

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var correlationId = context.HttpContext.GetCorrelationId() ?? context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["correlationId"] = correlationId;

                if (context.HttpContext.RequestServices.GetService<IWebHostEnvironment>()?.EnvironmentName == "Development")
                {
                    context.ProblemDetails.Extensions["stackTrace"] = context.Exception?.StackTrace ?? "No stack trace available";
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger/OpenAPI documentation with custom schema generation and filters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();

        return services;
    }

    /// <summary>
    /// Configures CORS policies for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>();

                if (allowedOrigins is { Length: > 0 })
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        return services;
    }
}