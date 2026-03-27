using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Amendments API client interface for managing amendments in the core platform.
/// </summary>
public interface IAmendmentsApiClient
{
    /// <summary>
    /// Gets all amendments for a specific idea.
    /// </summary>
    /// <param name="ideaId">The identifier of the idea to get amendments for.</param>
    /// <param name="query">The query specification for pagination and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of amendments for the specified idea.</returns>
    Task<PaginatedResponse<AmendmentsListItemResponse>> GetAmendmentsAsync(
        string ideaId,
        PaginationQuerySpec<AmendmentsQueryFilter> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets detailed information about a specific amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amendment details if found; otherwise, null.</returns>
    Task<AmendmentResponse> GetAmendmentDetailsAsync(
        string amendmentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets supporters for a specific amendment with pagination support.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to get supporters for.</param>
    /// <param name="query">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of supporters for the specified amendment.</returns>
    Task<PaginatedResponse<AmendmentSupporterResponse>> GetAmendmentSupportersAsync(
        string amendmentId,
        PaginationQuerySpec<object> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets opponents for a specific amendment with pagination support.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to get opponents for.</param>
    /// <param name="query">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of opponents for the specified amendment.</returns>
    Task<PaginatedResponse<AmendmentOpponentResponse>> GetAmendmentOpponentsAsync(
        string amendmentId,
        PaginationQuerySpec<object> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets support trend data for a specific amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to get trend data for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Support trend data for the specified amendment.</returns>
    Task<AmendmentTrendResponse> GetAmendmentTrendAsync(
        string amendmentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Proposes a new amendment for an idea.
    /// </summary>
    /// <param name="ideaId">The identifier of the idea to propose an amendment for.</param>
    /// <param name="request">The amendment proposal request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The proposed amendment response.</returns>
    Task<ProposeAmendmentResponse> ProposeAmendmentAsync(
        string ideaId,
        ProposeAmendmentRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds support for an amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to support.</param>
    /// <param name="request">The request containing the reason for supporting the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    Task<AmendmentVoteResponse> SupportAmendmentAsync(
        string amendmentId,
        SupportAmendmentRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds opposition to an amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to oppose.</param>
    /// <param name="request">The request containing the reason for opposing the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    Task<AmendmentVoteResponse> OpposeAmendmentAsync(
        string amendmentId,
        OpposeAmendmentRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Withdraws support for an amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to withdraw support from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    Task<AmendmentVoteResponse> WithdrawSupportAsync(
        string amendmentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Withdraws opposition to an amendment.
    /// </summary>
    /// <param name="amendmentId">The identifier of the amendment to withdraw opposition from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    Task<AmendmentVoteResponse> WithdrawOppositionAsync(
        string amendmentId,
        CancellationToken cancellationToken);
}
