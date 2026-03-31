using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Signals;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller responsible for handling concern-related API requests.
/// </summary>
/// <param name="messageBus">A message bus used to send commands and events.</param>
[ApiController]
[Route("api/concerns")]
public sealed class ConcernsController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Handles a request to raise a new concern.
    /// </summary>
    /// <param name="request">Raise concern request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Id of the raised concern.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("raiseConcern")]
    public async Task<IActionResult> RaiseConcern(
        [FromBody] RaiseConcernRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var actorId = HttpContext.GetParticipantIdOrAnonymous();
        var command = new RaiseConcern(
            actorId,
            request.Name,
            request.Tags,
            [..request.SignalIds.Select(SignalId.Create)]);

        var concernId = await messageBus
            .InvokeAsync<ConcernId>(command, cancellationToken)
            .ConfigureAwait(false);
        return Created(string.Empty, concernId.Value);
    }
}