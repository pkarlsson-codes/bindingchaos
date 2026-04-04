using BindingChaos.Infrastructure.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Extension methods for registering API clients in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Signals API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddSignalsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services
            .AddHttpClient<ISignalsApiClient, SignalsApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Ideas API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddIdeasApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IIdeasApiClient, IdeasApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Discourse API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddDiscourseApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IDiscourseApiClient, DiscourseApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Concerns API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddConcernsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IConcernsApiClient, ConcernsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Documents API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddDocumentsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IDocumentsApiClient, DocumentsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Tags API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddTagsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<ITagsApiClient, TagsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Societies API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddSocietiesApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<ISocietiesApiClient, SocietiesApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Emerging Patterns API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddEmergingPatternsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IEmergingPatternsApiClient, EmergingPatternsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Commons API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddCommonsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<ICommonsApiClient, CommonsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the User Groups API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddUserGroupsApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<IUserGroupsApiClient, UserGroupsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }

    /// <summary>
    /// Adds the Invite Links API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddTrustInviteLinksApiClient(
        this IServiceCollection services,
        string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        return services.AddHttpClient<ITrustInviteLinksApiClient, TrustInviteLinksApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<CorrelationIdHandler>();
    }
}