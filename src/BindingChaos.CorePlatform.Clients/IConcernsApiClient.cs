using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client responsible for interacting with the concerns API endpoints.
/// </summary>
public interface IConcernsApiClient
{
    /// <summary>
    /// Raises a new concern by sending a request to the concerns API.
    /// </summary>
    /// <param name="request">The request containing the details of the concern to raise.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the raised concern as a string.</returns>
    Task<string> RaiseConcernAsync(RaiseConcernRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of concerns from the concerns API endpoint.
    /// </summary>
    /// <param name="querySpec">The pagination and filtering specification for the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of concerns.</returns>
    Task<PaginatedResponse<ConcernListItemResponse>> GetConcernsAsync(
        PaginationQuerySpec<ConcernsQueryFilter> querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Declares that the current participant is affected by the specified concern.
    /// </summary>
    /// <param name="concernId">The ID of the concern.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeclareAffectedAsync(string concernId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Withdraws the current participant's affectedness declaration for the specified concern.
    /// </summary>
    /// <param name="concernId">The ID of the concern.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task WithdrawAffectednessAsync(string concernId, CancellationToken cancellationToken = default);
}
