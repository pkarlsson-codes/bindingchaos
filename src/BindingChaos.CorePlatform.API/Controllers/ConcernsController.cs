using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Concerns;
using BindingChaos.Stigmergy.Domain.Signals;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller responsible for handling concern-related API requests.
/// </summary>
/// <param name="messageBus">A message bus used to send commands and events.</param>
/// <param name="pseudonymLookupService">A service used to resolve user pseudonyms from participant identifiers.</param>
[ApiController]
[Route("api/concerns")]
public sealed class ConcernsController(
    IMessageBus messageBus,
    IPseudonymLookupService pseudonymLookupService)
    : BaseApiController
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
            .InvokeAsync<ConcernId>(command, cancellationToken);
        return Created(string.Empty, concernId.Value);
    }

    /// <summary>
    /// Retrieves all currently tracked concerns.
    /// </summary>
    /// <param name="queryRequest">Pagination and sorting parameters from the querystring.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all concerns.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ConcernListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getConcerns")]
    public async Task<IActionResult> GetConcerns(
        [FromQuery] PaginationQuerySpec queryRequest,
        CancellationToken cancellationToken)
    {
        var concerns = await messageBus
            .InvokeAsync<PaginatedResponse<ConcernsListItemView>>(new GetConcerns(queryRequest), cancellationToken)
            .ConfigureAwait(false);

        var pseudonyms = await pseudonymLookupService.GetPseudonymsAsync(
            concerns.Items.Select(c => c.RaisedById),
            cancellationToken);

        var response = concerns.MapItems(
            c => new ConcernListItemResponse(
                c.Id,
                pseudonyms.GetValueOrDefault(c.RaisedById, "Anonymous"),
                c.Name,
                c.Tags,
                [..c.Signals.Select(s => new ConcernListItemResponse.ReferenceSignal(s.Id, s.Title))]));

        return Ok(response);
    }
}