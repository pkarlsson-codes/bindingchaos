using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// OpenAPI/NSwag registration helpers.
/// </summary>
public static class OpenApiServiceCollectionExtensions
{
    /// <summary>
    /// Registers OpenAPI document and explorer with development servers.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">Application configuration, used to populate server URLs.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(options =>
        {
            options.DocumentName = "v1";
            options.Title = "BindingChaos.Web.Gateway";
            options.Version = "1.0";
            options.Description = "BindingChaos Web Gateway API";
            options.OperationProcessors.Add(new EndpointNameOperationProcessor());
            options.PostProcess = document =>
            {
                var httpUrl = configuration["Gateway:BaseUrl"]
                    ?? configuration["Kestrel:Endpoints:Http:Url"]
                    ?? "http://localhost:4000";

                var httpsUrl = configuration["Kestrel:Endpoints:Https:Url"]
                    ?? "https://localhost:4001";

                document.Servers.Clear();
                document.Servers.Add(new NSwag.OpenApiServer { Url = httpUrl, Description = "HTTP dev (default)" });
                document.Servers.Add(new NSwag.OpenApiServer { Url = httpsUrl, Description = "HTTPS dev" });
            };
        });
        return services;
    }

    /// <summary>
    /// NSwag operation processor that uses <see cref="EndpointNameAttribute"/> as the operation ID,
    /// matching the explicit names set on each controller action.
    /// </summary>
    private sealed class EndpointNameOperationProcessor : IOperationProcessor
    {
        /// <inheritdoc/>
        public bool Process(OperationProcessorContext context)
        {
            var endpointName = context.MethodInfo
                .GetCustomAttributes(typeof(EndpointNameAttribute), inherit: false)
                .OfType<EndpointNameAttribute>()
                .FirstOrDefault();

            if (endpointName is not null)
            {
                context.OperationDescription.Operation.OperationId = endpointName.EndpointName;
            }

            return true;
        }
    }
}
