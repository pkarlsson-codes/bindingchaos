using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Application.Queries;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Provides endpoints for managing amendments related to a specific idea.
/// </summary>
[ApiController]
[Route("api")]
public sealed class AmendmentsController(IMessageBus messageBus, IPseudonymLookupService pseudonymService) : BaseApiController
{
    /// <summary>
    /// Proposes a new amendment for an idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea to propose an amendment for.</param>
    /// <param name="request">The request containing the proposed title and body for the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A created response with the ID of the proposed amendment.</returns>
    [HttpPost("ideas/{ideaId}/amendments")]
    [ProducesResponseType(typeof(ApiResponse<ProposeAmendmentResponse>), 201)]
    [EndpointName("proposeAmendment")]
    public async Task<IActionResult> ProposeAmendment(string ideaId, [FromBody] ProposeAmendmentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        var command = new ProposeAmendment(
            IdeaId.Create(ideaId),
            request.TargetIdeaVersion,
            participantId,
            request.ProposedTitle,
            request.ProposedBody,
            request.AmendmentTitle,
            request.AmendmentDescription);

        var amendmentId = await messageBus.InvokeAsync<AmendmentId>(command, cancellationToken).ConfigureAwait(false);
        var response = new ProposeAmendmentResponse(amendmentId.Value);
        return CreatedAtAction(nameof(GetAmendments), new { ideaId }, response);
    }

    /// <summary>
    /// Gets all amendments for a specific idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea to retrieve amendments for.</param>
    /// <param name="request">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of amendments for the specified idea.</returns>
    [HttpGet("ideas/{ideaId}/amendments")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentsListItemResponse>>), 200)]
    [EndpointName("getAmendments")]
    public async Task<IActionResult> GetAmendments([FromRoute] string ideaId, [FromQuery] PaginationQuerySpec<AmendmentsQueryFilter> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Filter.IdeaId = ideaId;
        var query = new GetAmendments(request);

        var viewResult = await messageBus.InvokeAsync<PaginatedResponse<AmendmentsListItemView>>(query, cancellationToken).ConfigureAwait(false);

        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();

        var authorIds = viewResult.Items.Select(a => a.AuthorId);
        var pseudonyms = await pseudonymService.GetPseudonymsAsync(authorIds, cancellationToken).ConfigureAwait(false);

        var response = viewResult.MapItems(amendment =>
        {
            pseudonyms.TryGetValue(amendment.AuthorId, out var authorPseudonym);
            return AmendmentMapper.ToAmendmentsListItemResponse(amendment, currentParticipantId, authorPseudonym ?? amendment.AuthorId);
        });

        return Ok(response);
    }

    /// <summary>
    /// Gets a specific amendment by its ID.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amendment details if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("amendments/{amendmentId}")]
    [ProducesResponseType(typeof(ApiResponse<AmendmentResponse>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendment")]
    public async Task<IActionResult> GetAmendment([FromRoute] string amendmentId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        var amendment = await GetAmendmentOrDefault(amendmentId, cancellationToken).ConfigureAwait(false);
        if (amendment is null)
        {
            return NotFound();
        }

        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();
        var creatorPseudonym = await pseudonymService.GetPseudonymAsync(amendment.CreatorId, cancellationToken).ConfigureAwait(false) ?? amendment.CreatorId;
        var response = AmendmentMapper.ToAmendmentResponse(amendment, currentParticipantId, creatorPseudonym);

        return Ok(response);
    }

    /// <summary>
    /// Gets supporters for a specific amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to get supporters for.</param>
    /// <param name="request">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of supporters for the specified amendment.</returns>
    [HttpGet("amendments/{amendmentId}/supporters")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentSupporterResponse>>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentSupporters")]
    public async Task<IActionResult> GetAmendmentSupporters([FromRoute] string amendmentId, [FromQuery] PaginationQuerySpec<object> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var amendment = await GetAmendmentOrDefault(amendmentId, cancellationToken).ConfigureAwait(false);
        if (amendment is null)
        {
            return NotFound();
        }

        var supportersQuery = new GetAmendmentSupporters(amendmentId, request);
        var supportersResult = await messageBus.InvokeAsync<PaginatedResponse<AmendmentSupporterView>>(supportersQuery, cancellationToken).ConfigureAwait(false);

        var participantIds = supportersResult.Items.Select(s => s.ParticipantId);
        var pseudonyms = await pseudonymService.GetPseudonymsAsync(participantIds, cancellationToken).ConfigureAwait(false);

        return Ok(supportersResult.MapItems(s => AmendmentMapper.ToAmendmentSupporterResponse(s, pseudonyms)));
    }

    /// <summary>
    /// Gets opponents for a specific amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to get opponents for.</param>
    /// <param name="request">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of opponents for the specified amendment.</returns>
    [HttpGet("amendments/{amendmentId}/opponents")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentOpponentResponse>>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentOpponents")]
    public async Task<IActionResult> GetAmendmentOpponents([FromRoute] string amendmentId, [FromQuery] PaginationQuerySpec<object> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var amendment = await GetAmendmentOrDefault(amendmentId, cancellationToken).ConfigureAwait(false);
        if (amendment is null)
        {
            return NotFound();
        }

        var opponentsQuery = new GetAmendmentOpponents(amendmentId, request);
        var opponentsResult = await messageBus.InvokeAsync<PaginatedResponse<AmendmentOpponentView>>(opponentsQuery, cancellationToken).ConfigureAwait(false);

        var participantIds = opponentsResult.Items.Select(o => o.ParticipantId);
        var pseudonyms = await pseudonymService.GetPseudonymsAsync(participantIds, cancellationToken).ConfigureAwait(false);

        return Ok(opponentsResult.MapItems(o => AmendmentMapper.ToAmendmentOpponentResponse(o, pseudonyms)));
    }

    /// <summary>
    /// Gets support trend data for a specific amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to get trend data for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The support trend data for the specified amendment.</returns>
    [HttpGet("amendments/{amendmentId}/trend")]
    [ProducesResponseType(typeof(ApiResponse<AmendmentTrendResponse>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentTrend")]
    public async Task<IActionResult> GetAmendmentTrend([FromRoute] string amendmentId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(amendmentId);

        var trendQuery = new GetAmendmentTrend(amendmentId);
        var trendResult = await messageBus.InvokeAsync<AmendmentTrendView?>(trendQuery, cancellationToken).ConfigureAwait(false);

        if (trendResult == null)
        {
            return NotFound();
        }

        return Ok(AmendmentMapper.ToAmendmentTrendResponse(trendResult));
    }

    /// <summary>
    /// Adds support for an amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to support.</param>
    /// <param name="request">The request containing the reason for supporting the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("amendments/{amendmentId}/supporters")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("supportAmendment")]
    public async Task<IActionResult> SupportAmendment(string amendmentId, [FromBody] SupportAmendmentRequest request, CancellationToken cancellationToken)
    {
        var command = new SupportAmendment(
            AmendmentId.Create(amendmentId),
            HttpContext.GetParticipantIdOrAnonymous(),
            request.Reason);

        var result = await messageBus.InvokeAsync<AmendmentSupportCounts>(command, cancellationToken).ConfigureAwait(false);

        return Ok(new AmendmentVoteResponse(result.SupporterCount, result.OpponentCount));
    }

    /// <summary>
    /// Adds opposition to an amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to oppose.</param>
    /// <param name="request">The request containing the reason for opposing the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("amendments/{amendmentId}/detractors")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("opposeAmendment")]
    public async Task<IActionResult> OpposeAmendment(string amendmentId, [FromBody] OpposeAmendmentRequest request, CancellationToken cancellationToken)
    {
        var command = new OpposeAmendment(
            AmendmentId.Create(amendmentId),
            HttpContext.GetParticipantIdOrAnonymous(),
            request.Reason);

        var result = await messageBus.InvokeAsync<AmendmentSupportCounts>(command, cancellationToken).ConfigureAwait(false);

        return Ok(new AmendmentVoteResponse(result.SupporterCount, result.OpponentCount));
    }

    /// <summary>
    /// Withdraws support for an amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to withdraw support from.</param>
    /// <param name="request">The request for withdrawing support (no additional data required).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpDelete("amendments/{amendmentId}/supporters")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("withdrawSupport")]
    public async Task<IActionResult> WithdrawSupport(string amendmentId, [FromBody] WithdrawSupportRequest request, CancellationToken cancellationToken)
    {
        var command = new WithdrawSupport(
            AmendmentId.Create(amendmentId),
            HttpContext.GetParticipantIdOrAnonymous());

        var result = await messageBus.InvokeAsync<AmendmentSupportCounts>(command, cancellationToken).ConfigureAwait(false);

        return Ok(new AmendmentVoteResponse(result.SupporterCount, result.OpponentCount));
    }

    /// <summary>
    /// Withdraws opposition to an amendment.
    /// </summary>
    /// <param name="amendmentId">The ID of the amendment to withdraw opposition from.</param>
    /// <param name="request">The request for withdrawing opposition (no additional data required).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("amendments/{amendmentId}/withdraw-opposition")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("withdrawOpposition")]
    public async Task<IActionResult> WithdrawOpposition(string amendmentId, [FromBody] WithdrawOppositionRequest request, CancellationToken cancellationToken)
    {
        var command = new WithdrawOpposition(
            AmendmentId.Create(amendmentId),
            HttpContext.GetParticipantIdOrAnonymous());

        var result = await messageBus.InvokeAsync<AmendmentSupportCounts>(command, cancellationToken).ConfigureAwait(false);

        return Ok(new AmendmentVoteResponse(result.SupporterCount, result.OpponentCount));
    }

    private async Task<AmendmentDetailView?> GetAmendmentOrDefault(string amendmentId, CancellationToken cancellationToken)
    {
        return await messageBus.InvokeAsync<AmendmentDetailView?>(new GetAmendment(amendmentId), cancellationToken).ConfigureAwait(false);
    }
}
