using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller responsible for handling requests related to user groups and forwarding them to the user groups API client.
/// </summary>
/// <param name="userGroupsApiClient">The API client used to forward user group requests to the core platform API.</param>
[ApiController]
[Route("api/v1/usergroups")]
public sealed class UserGroupsController(IUserGroupsApiClient userGroupsApiClient) : BaseApiController
{
    /// <summary>
    /// Forwards a request to form a new user group to the user groups API.
    /// </summary>
    /// <param name="request">The request containing the details of the user group to form.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the formed user group.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("formUserGroup")]
    public async Task<IActionResult> FormUserGroup(
        [FromBody] FormUserGroupRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userGroupId = await userGroupsApiClient
            .FormUserGroupAsync(request, cancellationToken);

        return Created(string.Empty, userGroupId);
    }

    /// <summary>
    /// Retrieves a paginated list of user groups governing the specified commons.
    /// </summary>
    /// <param name="commonsId">The ID of the commons to filter by.</param>
    /// <param name="querySpec">The pagination and sorting specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of user groups.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserGroupListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getUserGroupsForCommons")]
    public async Task<IActionResult> GetUserGroupsForCommons(
        [FromQuery] string commonsId,
        [FromQuery] PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var userGroups = await userGroupsApiClient
            .GetUserGroupsForCommonsAsync(commonsId, querySpec, cancellationToken);

        return Ok(userGroups);
    }

    /// <summary>
    /// Retrieves all user groups that the authenticated participant is a member of.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of user groups the current participant belongs to.</returns>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserGroupListItemResponse>>), StatusCodes.Status200OK)]
    [EndpointName("getMyUserGroups")]
    public async Task<IActionResult> GetMyUserGroups(CancellationToken cancellationToken)
    {
        var result = await userGroupsApiClient.GetMyUserGroupsAsync(cancellationToken);
        return Ok(result);
    }
}
