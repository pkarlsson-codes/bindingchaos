using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Signals API client.
/// </summary>
/// <remarks>
/// Initializes a new instance of the SignalsApiClient class.
/// </remarks>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public class SignalsApiClient(
    HttpClient httpClient,
    ILogger<SignalsApiClient> logger)
    : BaseApiClient(httpClient, logger), ISignalsApiClient
{
    /// <inheritdoc/>
    public Task<SignalResponse> GetSignal(
        string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        return GetAsync<SignalResponse>(
            $"api/signals/{signalId}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PaginatedResponse<SignalListItemResponse>> GetSignals(
        PaginationQuerySpec<SignalsQueryFilter> querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var queryString = querySpec?.ToQueryString(true) ?? string.Empty;
        return GetAsync<PaginatedResponse<SignalListItemResponse>>(
            $"api/signals{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<CaptureSignalResponse> CaptureSignal(
        CaptureSignalRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<CaptureSignalRequest, CaptureSignalResponse>(
            "api/signals",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmplifySignalResponse> AmplifySignal(
        string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        return PostAsync<object, AmplifySignalResponse>(
            $"api/signals/{signalId}/amplifications",
            new { },
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<DeamplifySignalResponse> DeamplifySignal(
        string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        return DeleteAsync<DeamplifySignalResponse>(
            $"api/signals/{signalId}/amplifications",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SignalAmplificationTrendResponse> GetSignalAmplificationTrendAsync(
        string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        return GetAsync<SignalAmplificationTrendResponse>(
            $"api/signals/{signalId}/amplification-trend",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task SuggestAction(
        string signalId,
        SuggestActionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<SuggestActionRequest, object>(
            $"api/signals/{signalId}/suggested-actions",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<ActionTypeResponse>> GetActionTypes(
        CancellationToken cancellationToken)
    {
        return GetAsync<IEnumerable<ActionTypeResponse>>(
            "api/action-types",
            cancellationToken);
    }
}