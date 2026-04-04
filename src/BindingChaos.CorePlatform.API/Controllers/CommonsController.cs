using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using ProposeCommonsCommand = BindingChaos.Stigmergy.Application.Commands.ProposeCommons;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing commons.
/// </summary>
/// <param name="messageBus">The message bus for dispatching commands.</param>
/// <param name="pseudonymLookupService">A service used to resolve user pseudonyms from participant identifiers.</param>
[ApiController]
[Route("api/commons")]
public sealed class CommonsController(
    IMessageBus messageBus,
    IPseudonymLookupService pseudonymLookupService)
    : BaseApiController
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

    /// <summary>
    /// Retrieves all currently tracked commons.
    /// </summary>
    /// <param name="queryRequest">Pagination and sorting parameters from the querystring.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of all commons.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CommonsListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getCommons")]
    public async Task<IActionResult> GetCommons(
        [FromQuery] PaginationQuerySpec queryRequest,
        CancellationToken cancellationToken)
    {
        var commons = await messageBus
            .InvokeAsync<PaginatedResponse<CommonsListItemView>>(new GetCommons(queryRequest), cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymLookupService.GetPseudonymsAsync(
            commons.Items.Select(c => c.FounderId),
            cancellationToken);

        var response = commons.MapItems(
            c => new CommonsListItemResponse(
                c.Id,
                c.Name,
                c.Description,
                c.Status,
                pseudonyms.GetValueOrDefault(c.FounderId, "Anonymous"),
                c.ProposedAt));

        return Ok(response);
    }
}
