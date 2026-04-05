using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller responsible for handling requests related to concerns and forwarding them to the concerns API client.
/// </summary>
/// <param name="concernsApiClient">The API client used to forward concern-related requests to the concerns API.</param>
[ApiController]
[Route("api/v1/concerns")]
public sealed class ConcernsController(IConcernsApiClient concernsApiClient) : BaseApiController
{
    /// <summary>
    /// Forwards a request to raise a new concern to the concerns API.
    /// </summary>
    /// <param name="request">The request containing the details of the concern to raise.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the raised concern as a string.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("raiseConcern")]
    public async Task<IActionResult> RaiseConcern(
        [FromBody] RaiseConcernRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var concernId = await concernsApiClient
            .RaiseConcernAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return Created(string.Empty, concernId);
    }

    /// <summary>
    /// Retrieves a paginated list of concerns by forwarding the request to the concerns API client.
    /// </summary>
    /// <param name="querySpec">The pagination and filtering specification for the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing the list of concerns.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ConcernListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getConcerns")]
    public async Task<IActionResult> GetConcerns(
        [FromQuery] PaginationQuerySpec<ConcernsQueryFilter> querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var concerns = await concernsApiClient
            .GetConcernsAsync(querySpec, cancellationToken)
            .ConfigureAwait(false);

        return Ok(concerns);
    }

    /// <summary>
    /// Declares that the current participant is affected by the specified concern.
    /// </summary>
    /// <param name="concernId">The ID of the concern.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpPost("{concernId}/affectedness")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("declareAffected")]
    public async Task<IActionResult> DeclareAffected(
        [FromRoute] string concernId,
        CancellationToken cancellationToken)
    {
        await concernsApiClient
            .DeclareAffectedAsync(concernId, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Withdraws the current participant's affectedness declaration for the specified concern.
    /// </summary>
    /// <param name="concernId">The ID of the concern.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{concernId}/affectedness")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("withdrawAffectedness")]
    public async Task<IActionResult> WithdrawAffectedness(
        [FromRoute] string concernId,
        CancellationToken cancellationToken)
    {
        await concernsApiClient
            .WithdrawAffectednessAsync(concernId, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }
}