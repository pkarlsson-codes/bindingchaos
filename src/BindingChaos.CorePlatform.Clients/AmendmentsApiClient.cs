using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Amendments API client.
/// </summary>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public class AmendmentsApiClient(HttpClient httpClient, ILogger<AmendmentsApiClient> logger) : BaseApiClient(httpClient, logger), IAmendmentsApiClient
{
    /// <inheritdoc/>
    public async Task<PaginatedResponse<AmendmentsListItemResponse>> GetAmendmentsAsync(string ideaId, PaginationQuerySpec<AmendmentsQueryFilter> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(query);

        query.Filter.IdeaId = ideaId;

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<AmendmentsListItemResponse>>($"api/ideas/{ideaId}/amendments{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentResponse?> GetAmendmentDetailsAsync(string amendmentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return await GetAsync<AmendmentResponse>($"api/amendments/{amendmentId}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<AmendmentSupporterResponse>> GetAmendmentSupportersAsync(string amendmentId, PaginationQuerySpec<object> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<AmendmentSupporterResponse>>($"api/amendments/{amendmentId}/supporters{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<AmendmentOpponentResponse>> GetAmendmentOpponentsAsync(string amendmentId, PaginationQuerySpec<object> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<AmendmentOpponentResponse>>($"api/amendments/{amendmentId}/opponents{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentTrendResponse> GetAmendmentTrendAsync(string amendmentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return await GetAsync<AmendmentTrendResponse>($"api/amendments/{amendmentId}/trend", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ProposeAmendmentResponse> ProposeAmendmentAsync(string ideaId, ProposeAmendmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<ProposeAmendmentRequest, ProposeAmendmentResponse>(
                $"api/ideas/{ideaId}/amendments",
                request,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentVoteResponse> SupportAmendmentAsync(string amendmentId, SupportAmendmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<SupportAmendmentRequest, AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/supporters",
                request,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentVoteResponse> OpposeAmendmentAsync(string amendmentId, OpposeAmendmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<OpposeAmendmentRequest, AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/detractors",
                request,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentVoteResponse> WithdrawSupportAsync(string amendmentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return await DeleteAsync<AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/withdraw-support",
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<AmendmentVoteResponse> WithdrawOppositionAsync(string amendmentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return await DeleteAsync<AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/withdraw-opposition",
                cancellationToken)
            .ConfigureAwait(false);
    }
}