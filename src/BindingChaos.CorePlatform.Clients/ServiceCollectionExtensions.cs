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
    public static IHttpClientBuilder AddSignalsApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<ISignalsApiClient, SignalsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Ideas API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddIdeasApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<IIdeasApiClient, IdeasApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Amendments API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddAmendmentsApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<IAmendmentsApiClient, AmendmentsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Discourse API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddDiscourseApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<IDiscourseApiClient, DiscourseApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Documents API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddDocumentsApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<IDocumentsApiClient, DocumentsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Tags API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddTagsApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<ITagsApiClient, TagsApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Societies API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddSocietiesApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<ISocietiesApiClient, SocietiesApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    /// <summary>
    /// Adds the Invite Links API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IHttpClientBuilder AddTrustTrustInviteLinksApiClient(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        services.TryAddScoped<CorrelationIdHandler>();

        var builder = services.AddHttpClient<ITrustTrustInviteLinksApiClient, TrustTrustInviteLinksApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        builder.AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }
}