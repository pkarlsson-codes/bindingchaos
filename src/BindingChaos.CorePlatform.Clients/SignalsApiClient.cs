using System.Net.Http.Json;
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
public class SignalsApiClient : BaseApiClient, ISignalsApiClient
{
    /// <summary>
    /// Initializes a new instance of the SignalsApiClient class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API requests.</param>
    /// <param name="logger">The logger for this client.</param>
    public SignalsApiClient(HttpClient httpClient, ILogger<SignalsApiClient> logger)
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Retrieves a specific signal by its ID.
    /// </summary>
    /// <param name="signalId">The ID of the signal to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns> A task that represents the asynchronous operation. The task result contains the SignalContract for the specified signal.</returns>
    public async Task<SignalResponse> GetSignal(string signalId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        return await GetAsync<SignalResponse>($"api/signals/{signalId}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<SignalListItemResponse>> GetSignals(PaginationQuerySpec<SignalsQueryFilter> querySpec, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var queryString = querySpec?.ToQueryString(true) ?? string.Empty;
        return await GetAsync<PaginatedResponse<SignalListItemResponse>>($"api/signals{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<CaptureSignalResponse> CaptureSignal(CaptureSignalRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<CaptureSignalRequest, CaptureSignalResponse>("api/signals", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmplifySignalResponse> AmplifySignal(string signalId, AmplifySignalRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<AmplifySignalRequest, AmplifySignalResponse>($"api/signals/{signalId}/amplifications", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<DeamplifySignalResponse> DeamplifySignal(string signalId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        return DeleteAsync<DeamplifySignalResponse>($"api/signals/{signalId}/amplifications", cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SignalAmplificationTrendResponse> GetSignalAmplificationTrendAsync(string signalId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        return await GetAsync<SignalAmplificationTrendResponse>($"api/signals/{signalId}/amplification-trend", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SuggestAction(string signalId, SuggestActionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(request);

        var response = await HttpClient.PostAsJsonAsync($"api/signals/{signalId}/suggested-actions", request, JsonOptions, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ActionTypeResponse>> GetActionTypes(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<ActionTypeResponse>>("api/action-types", cancellationToken).ConfigureAwait(false);
    }
}