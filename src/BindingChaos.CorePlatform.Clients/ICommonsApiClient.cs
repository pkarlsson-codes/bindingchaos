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
}
