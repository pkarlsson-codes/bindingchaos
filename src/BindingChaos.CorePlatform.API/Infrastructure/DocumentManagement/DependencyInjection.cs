using BindingChaos.CorePlatform.API.Configuration;
using BindingChaos.CorePlatform.API.Services;
using Minio;

namespace BindingChaos.CorePlatform.API.Infrastructure.DocumentManagement;

/// <summary>
/// Dependency injection configuration for document management services.
/// </summary>
public static class DocumentManagementServiceCollectionExtensions
{
    /// <summary>
    /// Adds document management services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddDocumentManagement(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<MinioOptions>(configuration.GetSection("Minio"));

        var minioOptions = configuration.GetSection("Minio").Get<MinioOptions>() ?? new MinioOptions();

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioOptions.Endpoint)
            .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
            .WithSSL(minioOptions.UseSsl)
            .Build());

        services.AddScoped<IDocumentManagementService, MinioDocumentManagementService>();

        return services;
    }
}
