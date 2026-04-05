using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client responsible for interacting with the commons API endpoints.
/// </summary>
public interface ICommonsApiClient
{
    /// <summary>
    /// Retrieves a paginated list of commons from the commons API endpoint.
    /// </summary>
    /// <param name="querySpec">The pagination and filtering specification for the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of commons.</returns>
    Task<PaginatedResponse<CommonsListItemResponse>> GetCommonsAsync(
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Proposes a new commons.
    /// </summary>
    /// <param name="request">The request containing the name and description of the commons.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the newly proposed commons.</returns>
    Task<string> ProposeCommonsAsync(
        ProposeCommonsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Links a concern to the specified commons.
    /// </summary>
    /// <param name="commonsId">The ID of the commons.</param>
    /// <param name="concernId">The ID of the concern to link.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LinkConcernToCommonsAsync(
        string commonsId,
        string concernId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all concerns linked to the specified commons.
    /// </summary>
    /// <param name="commonsId">The ID of the commons.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of concerns linked to the commons.</returns>
    Task<IReadOnlyList<ConcernListItemResponse>> GetConcernsForCommonsAsync(
        string commonsId,
        CancellationToken cancellationToken = default);
}
