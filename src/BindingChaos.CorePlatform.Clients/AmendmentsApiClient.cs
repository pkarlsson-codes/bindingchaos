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
public class AmendmentsApiClient(
    HttpClient httpClient,
    ILogger<AmendmentsApiClient> logger)
    : BaseApiClient(httpClient, logger), IAmendmentsApiClient
{
    /// <inheritdoc/>
    public Task<PaginatedResponse<AmendmentsListItemResponse>> GetAmendmentsAsync(
        string ideaId,
        PaginationQuerySpec<AmendmentsQueryFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(query);

        query.Filter.IdeaId = ideaId;

        var queryString = query.ToQueryString(true);
        return GetAsync<PaginatedResponse<AmendmentsListItemResponse>>(
            $"api/ideas/{ideaId}/amendments{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentResponse> GetAmendmentDetailsAsync(
        string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return GetAsync<AmendmentResponse>(
            $"api/amendments/{amendmentId}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PaginatedResponse<AmendmentSupporterResponse>> GetAmendmentSupportersAsync(
        string amendmentId,
        PaginationQuerySpec<object> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return GetAsync<PaginatedResponse<AmendmentSupporterResponse>>(
            $"api/amendments/{amendmentId}/supporters{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PaginatedResponse<AmendmentOpponentResponse>> GetAmendmentOpponentsAsync(
        string amendmentId,
        PaginationQuerySpec<object> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return GetAsync<PaginatedResponse<AmendmentOpponentResponse>>(
            $"api/amendments/{amendmentId}/opponents{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentTrendResponse> GetAmendmentTrendAsync(
        string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return GetAsync<AmendmentTrendResponse>(
            $"api/amendments/{amendmentId}/trend",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<ProposeAmendmentResponse> ProposeAmendmentAsync(
        string ideaId,
        ProposeAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<ProposeAmendmentRequest, ProposeAmendmentResponse>(
            $"api/ideas/{ideaId}/amendments",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentVoteResponse> SupportAmendmentAsync(
        string amendmentId,
        SupportAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<SupportAmendmentRequest, AmendmentVoteResponse>(
            $"api/amendments/{amendmentId}/supporters",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentVoteResponse> OpposeAmendmentAsync(
        string amendmentId,
        OpposeAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<OpposeAmendmentRequest, AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/detractors",
                request,
                cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentVoteResponse> WithdrawSupportAsync(
        string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return DeleteAsync<AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/withdraw-support",
                cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AmendmentVoteResponse> WithdrawOppositionAsync(
        string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        return DeleteAsync<AmendmentVoteResponse>(
                $"api/amendments/{amendmentId}/withdraw-opposition",
                cancellationToken);
    }
}