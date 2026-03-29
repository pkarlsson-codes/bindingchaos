using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Signals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing signals.
/// </summary>
/// <param name="messageBus">The message bus for handling asynchronous messaging.</param>
/// <param name="pseudonymService">The pseudonym lookup service for resolving participant identities.</param>
[ApiController]
[Route("api/signals")]
public sealed class SignalsController(IMessageBus messageBus, IPseudonymLookupService pseudonymService) : BaseApiController
{
    /// <summary>
    /// Gets a specific signal by its ID.
    /// </summary>
    /// <param name="signalIdString">The ID of the signal to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A signal view model containing details about the signal.</returns>
    [HttpGet("{signalId}")]
    [ProducesResponseType(typeof(ApiResponse<SignalResponse>), 200)]
    [EndpointName("getSignal")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignal(
        [FromRoute(Name = "signalId")] string signalIdString,
        CancellationToken cancellationToken)
    {
        var signalId = SignalId.Create(signalIdString);
        var query = new GetSignal(signalId);
        var signal = await messageBus.InvokeAsync<SignalView?>(query, cancellationToken).ConfigureAwait(false);
        if (signal == null)
        {
            return NotFound($"Signal with ID {signalIdString} not found.");
        }

        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();

        var participantIds = signal.Amplifications.Select(a => a.AmplifiedById).Append(signal.CapturedById);
        var pseudonyms = await pseudonymService.GetPseudonymsAsync(participantIds, cancellationToken).ConfigureAwait(false);

        var contract = SignalMapper.ToSignalResponse(signal, currentParticipantId, pseudonyms);
        return Ok(contract);
    }

    /// <summary>
    /// Gets all signals.
    /// </summary>
    /// <param name="queryRequest">The signals query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of signals.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SignalListItemResponse>>), 200)]
    [EndpointName("getSignals")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignals(
        [FromQuery] PaginationQuerySpec<SignalsQueryFilter> queryRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(queryRequest);

        var query = new GetSignals(queryRequest);
        var signalsPage = await messageBus.InvokeAsync<PaginatedResponse<SignalsListItemView>>(query, cancellationToken).ConfigureAwait(false);

        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();

        var originatorIds = signalsPage.Items.Select(s => s.CapturedById);
        var pseudonyms = await pseudonymService.GetPseudonymsAsync(originatorIds, cancellationToken).ConfigureAwait(false);

        var response = signalsPage.MapItems(signal =>
        {
            pseudonyms.TryGetValue(signal.CapturedById, out var originatorPseudonym);
            return SignalMapper.ToSignalListItemResponse(signal, currentParticipantId, originatorPseudonym ?? signal.CapturedById);
        });

        return Ok(response);
    }

    /// <summary>
    /// Creates a new signal.
    /// </summary>
    /// <param name="request">The signal creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created signal.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("captureSignal")]
    [AllowAnonymous]
    public async Task<IActionResult> CaptureSignal([FromBody] CaptureSignalRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        var actorId = HttpContext.GetParticipantIdOrAnonymous();

        Coordinates? location = request.Latitude.HasValue && request.Longitude.HasValue
            ? new Coordinates(request.Latitude.Value, request.Longitude.Value)
            : null;

        var command = new CaptureSignal(
            actorId,
            request.Title,
            request.Description,
            [.. request.Tags],
            [.. request.AttachmentIds],
            location);

        var signalId = await messageBus.InvokeAsync<SignalId>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), new { signalId }, signalId);
    }

    /// <summary>
    /// Amplifies a signal by the current participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to amplify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amplification response.</returns>
    [HttpPost("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<AmplifySignalResponse>), 200)]
    [EndpointName("amplifySignal")]
    public async Task<IActionResult> AmplifySignal(string signalId, CancellationToken cancellationToken)
    {
        var amplifierId = HttpContext.GetParticipantIdOrAnonymous();
        if (amplifierId.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot amplify signals.");
        }

        var command = new AmplifySignal(amplifierId, SignalId.Create(signalId));
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), new { signalId }, new AmplifySignalResponse(0));
    }

    /// <summary>
    /// Removes the current participant's amplification from a signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to withdraw amplification from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The withdrawal response.</returns>
    [HttpDelete("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<DeamplifySignalResponse>), 200)]
    [EndpointName("deamplifySignal")]
    public async Task<IActionResult> WithdrawAmplification(string signalId, CancellationToken cancellationToken)
    {
        var amplifierId = HttpContext.GetParticipantIdOrAnonymous();
        if (amplifierId.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot withdraw amplification.");
        }

        var command = new WithdrawAmplification(amplifierId, SignalId.Create(signalId));
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(new DeamplifySignalResponse(0));
    }
}
