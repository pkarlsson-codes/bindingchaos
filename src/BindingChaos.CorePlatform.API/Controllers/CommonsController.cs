using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using ProposeCommonsCommand = BindingChaos.Stigmergy.Application.Commands.ProposeCommons;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing commons.
/// </summary>
/// <param name="messageBus">The message bus for dispatching commands.</param>
[ApiController]
[Route("api/commons")]
public sealed class CommonsController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Proposes a new commons.
    /// </summary>
    /// <param name="request">The proposal request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the proposed commons.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("proposeCommons")]
    public async Task<IActionResult> ProposeCommons([FromBody] ProposeCommonsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new ProposeCommonsCommand(request.Name, request.Description, participantId);
        var commonsId = await messageBus.InvokeAsync<CommonsId>(command, cancellationToken).ConfigureAwait(false);

        return Created($"api/commons/{commonsId.Value}", commonsId.Value);
    }
}
