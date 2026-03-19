using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Pseudonymity.Application.Services;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.SignalAwareness.Application.Commands;
using BindingChaos.SignalAwareness.Application.DTOs;
using BindingChaos.SignalAwareness.Application.Queries;
using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing signals.
/// </summary>
/// <param name="messageBus">The message bus for handling asynchronous messaging.</param>
/// <param name="pseudonymService">The pseudonym service for resolving participant identities.</param>
[ApiController]
[Route("api/signals")]
public sealed class SignalsController(IMessageBus messageBus, IPseudonymService pseudonymService) : BaseApiController
{
    /// <summary>
    /// Gets a specific signal by its ID.
    /// </summary>
    /// <param name="signalIdString">The ID of the signal to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns> A signal view model containing details about the signal.</returns>
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

        var participantIds = signal.Amplifications.Select(a => a.AmplifierId)
            .Concat(signal.SuggestedActions.Select(a => a.SuggestedById))
            .Append(signal.OriginatorId);
        var pseudonyms = pseudonymService.Generate(signalId, participantIds);

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

        var response = signalsPage.MapItems(signal =>
        {
            var originatorPseudonym = pseudonymService.Generate(SignalId.Create(signal.Id), signal.OriginatorId);
            return SignalMapper.ToSignalListItemResponse(signal, currentParticipantId, originatorPseudonym);
        });

        return Ok(response);
    }

    /// <summary>
    /// Creates a new signal.
    /// </summary>
    /// <param name="request">The signal creation request.</param>
    /// <returns>The created signal.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("captureSignal")]
    [AllowAnonymous]
    public async Task<IActionResult> CaptureSignal([FromBody] CaptureSignalRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        var originatorId = HttpContext.GetParticipantIdOrAnonymous();

        Coordinates? location = request.Latitude.HasValue && request.Longitude.HasValue
            ? new Coordinates(request.Latitude.Value, request.Longitude.Value)
            : null;

        var command = new CaptureSignal(
            request.Title,
            request.Description,
            originatorId,
            location,
            [.. request.Tags],
            [.. request.Attachments.Select(a => new AttachmentDto(a.DocumentId, a.Caption))]);

        var signalId = await messageBus.InvokeAsync<SignalId>(command).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), new { signalId }, signalId);
    }

    /// <summary>
    /// Amplifies a signal by a participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to amplify.</param>
    /// <param name="request">The amplification request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated signal information.</returns>
    [HttpPost("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [EndpointName("amplifySignal")]
    public async Task<IActionResult> AmplifySignal(string signalId, [FromBody] AmplifySignalRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var signalIdValue = SignalId.Create(signalId);
        var amplifierId = HttpContext.GetParticipantIdOrAnonymous();
        if (amplifierId.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot amplify signals.");
        }

        var reason = request.Reason == null ? AmplificationReason.HighRelevance : AmplificationReason.FromDisplayName(request.Reason);

        var command = new AmplifySignal(signalIdValue, amplifierId, reason, request.Commentary);
        var numberOfActiveAmplifications = await messageBus.InvokeAsync<int>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), new { signalId }, new AmplifySignalResponse(numberOfActiveAmplifications));
    }

    /// <summary>
    /// Adds evidence to a signal.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal to add evidence to.</param>
    /// <param name="request">The evidence request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The if of the added evidence.</returns>
    [HttpPost("{signalId}/evidence")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [EndpointName("addEvidence")]
    public async Task<IActionResult> AddEvidence(string signalId, [FromBody] AddEvidenceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var addedBy = HttpContext.GetParticipantIdOrAnonymous();
        if (addedBy.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot add evidence.");
        }

        var command = new AddEvidence(SignalId.Create(signalId), request.DocumentIds, request.Description, addedBy);
        var evidenceId = await messageBus.InvokeAsync<string>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), evidenceId);
    }

    /// <summary>
    /// Removes an amplification from the specified signal.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal to deamplify. Cannot be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the unique identifier of the deamplified signal if the operation is
    /// successful.</returns>
    [HttpDelete("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [EndpointName("deamplifySignal")]
    public async Task<IActionResult> DeamplifySignal(string signalId, CancellationToken cancellationToken)
    {
        var signalIdValue = SignalId.Create(signalId);
        var amplifierId = HttpContext.GetParticipantIdOrAnonymous();
        if (amplifierId.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot deamplify signals.");
        }

        var command = new DeamplifySignal(signalIdValue, amplifierId);
        var numberOfActiveAmplifications = await messageBus.InvokeAsync<int>(command, cancellationToken).ConfigureAwait(false);
        return Ok(new DeamplifySignalResponse(numberOfActiveAmplifications));
    }

    /// <summary>
    /// Suggests an action on a signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to suggest an action on.</param>
    /// <param name="request">The action suggestion request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A 201 Created response.</returns>
    [HttpPost("{signalId}/suggested-actions")]
    [ProducesResponseType(201)]
    [EndpointName("suggestAction")]
    public async Task<IActionResult> SuggestAction(string signalId, [FromBody] SuggestActionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId.IsAnonymous)
        {
            return BadRequest("Anonymous participants cannot suggest actions.");
        }

        var actionType = ActionType.FromDisplayName(request.ActionType!);
        ActionParameters parameters;
        if (actionType == ActionType.MakeACall)
        {
            parameters = new MakeACallParameters(request.PhoneNumber!, request.Details);
        }
        else if (actionType == ActionType.VisitAWebpage)
        {
            parameters = new VisitAWebpageParameters(request.Url!, request.Details);
        }
        else
        {
            return BadRequest($"Action type '{request.ActionType}' is not supported.");
        }

        var signalIdValue = SignalId.Create(signalId);
        var command = new SuggestAction(signalIdValue, participantId, parameters);
        await messageBus.InvokeAsync<int>(command, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetSignal), new { signalId }, null);
    }

    /// <summary>
    /// Gets amplification trend data for a specific signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to get trend data for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The signal amplification trend data.</returns>
    [HttpGet("{signalId}/amplification-trend")]
    [ProducesResponseType(typeof(ApiResponse<SignalAmplificationTrendResponse>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getSignalAmplificationTrend")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignalAmplificationTrend([FromRoute] string signalId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var trendQuery = new GetSignalAmplificationTrend(signalId);
        var trendResult = await messageBus.InvokeAsync<SignalAmplificationTrendView?>(trendQuery, cancellationToken).ConfigureAwait(false);

        if (trendResult == null)
        {
            return NotFound();
        }

        var response = SignalMapper.ToSignalAmplificationTrendResponse(trendResult);
        return Ok(response);
    }
}
