using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller responsible for handling requests related to commons and forwarding them to the commons API client.
/// </summary>
/// <param name="commonsApiClient">The API client used to forward commons-related requests to the commons API.</param>
[ApiController]
[Route("api/v1/commons")]
public sealed class CommonsController(ICommonsApiClient commonsApiClient) : BaseApiController
{
    /// <summary>
    /// Forwards a request to propose a new commons to the commons API.
    /// </summary>
    /// <param name="request">The request containing the name and description of the commons to propose.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the proposed commons as a string.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("proposeCommons")]
    public async Task<IActionResult> ProposeCommons(
        [FromBody] ProposeCommonsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var commonsId = await commonsApiClient
            .ProposeCommonsAsync(request, cancellationToken);

        return Created(string.Empty, commonsId);
    }

    /// <summary>
    /// Retrieves a paginated list of commons by forwarding the request to the commons API client.
    /// </summary>
    /// <param name="querySpec">The pagination and filtering specification for the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of commons.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CommonsListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getCommons")]
    public async Task<IActionResult> GetCommons(
        [FromQuery] PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var commons = await commonsApiClient
            .GetCommonsAsync(querySpec, cancellationToken);

        return Ok(commons);
    }
}
