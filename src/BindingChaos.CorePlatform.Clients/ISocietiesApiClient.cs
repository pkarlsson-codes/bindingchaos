using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Societies API.
/// </summary>
public interface ISocietiesApiClient
{
    /// <summary>
    /// Gets a paginated list of societies with optional filtering.
    /// </summary>
    /// <param name="query">The query specification for pagination and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of society list items.</returns>
    Task<PaginatedResponse<SocietyListItemResponse>> GetSocietiesAsync(
        PaginationQuerySpec<SocietiesQueryFilter> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single society by its identifier.
    /// </summary>
    /// <param name="societyId">The identifier of the society.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The society response.</returns>
    Task<SocietyResponse> GetSocietyAsync(
        string societyId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the active members of a society.
    /// </summary>
    /// <param name="societyId">The identifier of the society.</param>
    /// <param name="query">The query specification for pagination.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of society member responses.</returns>
    Task<PaginatedResponse<SocietyMemberResponse>> GetSocietyMembersAsync(
        string societyId,
        PaginationQuerySpec<EmptyFilter> query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new society.
    /// </summary>
    /// <param name="request">The society creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created society.</returns>
    Task<string> CreateSocietyAsync(
        CreateSocietyRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Joins a society on behalf of the authenticated participant.
    /// </summary>
    /// <param name="societyId">The identifier of the society to join.</param>
    /// <param name="request">The join request containing the agreed social contract ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created membership.</returns>
    Task<string> JoinSocietyAsync(
        string societyId,
        JoinSocietyRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the IDs of all societies the authenticated participant is an active member of.
    /// Returns an empty array for unauthenticated requests.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of society ID strings.</returns>
    Task<string[]> GetMySocietyIdsAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes the authenticated participant's membership from a society.
    /// </summary>
    /// <param name="societyId">The identifier of the society to leave.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LeaveSocietyAsync(
        string societyId,
        CancellationToken cancellationToken);
}
