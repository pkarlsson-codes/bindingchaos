using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client responsible for interacting with the user groups API endpoints.
/// </summary>
public interface IUserGroupsApiClient
{
    /// <summary>
    /// Forms a new user group to govern the specified commons.
    /// </summary>
    /// <param name="request">The formation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the formed user group.</returns>
    Task<string> FormUserGroupAsync(
        FormUserGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of user groups governing the specified commons.
    /// </summary>
    /// <param name="commonsId">The ID of the commons to filter by.</param>
    /// <param name="querySpec">The pagination and sorting specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of user groups.</returns>
    Task<PaginatedResponse<UserGroupListItemResponse>> GetUserGroupsForCommonsAsync(
        string commonsId,
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all user groups that the authenticated participant is a member of.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of user groups the current participant belongs to.</returns>
    Task<IReadOnlyList<UserGroupListItemResponse>> GetMyUserGroupsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all user groups that the specified participant is a member of.
    /// </summary>
    /// <param name="participantId">The participant ID to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of user groups the participant belongs to.</returns>
    Task<UserGroupListItemResponse[]> GetUserGroupsForParticipantAsync(
        string participantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the detail of a user group by its ID.
    /// </summary>
    /// <param name="userGroupId">The ID of the user group.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user group detail, or <c>null</c> if not found.</returns>
    Task<UserGroupDetailResponse?> GetUserGroupDetailAsync(
        string userGroupId,
        CancellationToken cancellationToken = default);
}
