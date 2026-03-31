using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
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
}