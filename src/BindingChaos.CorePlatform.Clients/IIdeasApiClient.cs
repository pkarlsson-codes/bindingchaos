using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Ideas API.
/// </summary>
public interface IIdeasApiClient
{
    /// <summary>
    /// Gets all ideas for the current locality.
    /// </summary>
    /// <param name="query">The query specification for pagination and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of ideas.</returns>
    Task<PaginatedResponse<IdeaListItemResponse>> GetIdeasAsync(
        PaginationQuerySpec<IdeasQueryFilter> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a specific idea by its identifier in the current locality.
    /// </summary>
    /// <param name="ideaId">The identifier of the idea.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The idea response.</returns>
    Task<IdeaResponse> GetIdeaAsync(
        string ideaId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new idea in the current locality.
    /// </summary>
    /// <param name="request">The idea creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created idea.</returns>
    Task<string> AuthorIdeaAsync(
        AuthorIdeaRequest request,
        CancellationToken cancellationToken);
}