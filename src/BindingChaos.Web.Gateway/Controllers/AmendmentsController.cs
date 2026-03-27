using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing ideas in the web gateway.
/// </summary>
/// <param name="amendmentsApiClient">Client for interacting with the Amendments API.</param>
[ApiController]
[Route("api/v1/ideas/{ideaId}/amendments")]
public sealed class AmendmentsController(
    IAmendmentsApiClient amendmentsApiClient)
    : BaseApiController
{
    /// <summary>
    /// Gets all amendments for an idea with optional filtering and pagination.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea to retrieve amendments for.</param>
    /// <param name="query">Pagination and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Paginated list of ideas with metadata.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentsListItemResponse>>), 200)]
    [EndpointName("getAmendments")]
    public async Task<IActionResult> GetAmendments(
        [FromRoute] string ideaId,
        [FromQuery] PaginationQuerySpec<AmendmentsQueryFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(query);

        var amendmentsPage = await amendmentsApiClient
            .GetAmendmentsAsync(ideaId, query, cancellationToken);

        return Ok(amendmentsPage);
    }

    /// <summary>
    /// Gets detailed information about a specific amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Detailed amendment information if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{amendmentId}")]
    [ProducesResponseType(typeof(ApiResponse<AmendmentViewModel>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentDetails")]
    public async Task<IActionResult> GetAmendmentDetails(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(amendmentId);

        var amendment = await amendmentsApiClient
            .GetAmendmentDetailsAsync(amendmentId, cancellationToken);

        if (amendment == null)
        {
            return NotFound();
        }

        var viewModel = new AmendmentViewModel
        {
            Id = amendment.Id,
            Title = amendment.AmendmentTitle,
            ShortDescription = amendment.AmendmentDescription,
            ProposedTitle = amendment.ProposedTitle,
            ProposedBody = amendment.ProposedBody,
            ProposerPseudonym = amendment.CreatorPseudonym,
            ProposedByCurrentUser = amendment.CreatedByCurrentUser,
            Status = amendment.Status,
            TargetVersionNumber = amendment.TargetVersionNumber,
            PropsedAt = amendment.CreatedAt.ToString("O"),
            SupporterCount = amendment.SupporterCount,
            OpponentCount = amendment.OpponentCount,
            AcceptedOn = amendment.AcceptedAt?.ToString("O"),
            RejectedOn = amendment.RejectedAt?.ToString("O"),
            SupportedByCurrentUser = amendment.SupportedByCurrentUser,
            OpposedByCurrentUser = amendment.OpposedByCurrentUser,
        };

        return Ok(viewModel);
    }

    /// <summary>
    /// Gets supporters for a specific amendment with pagination support.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to get supporters for.</param>
    /// <param name="query">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of supporters for the specified amendment.</returns>
    [HttpGet("{amendmentId}/supporters")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentSupporterResponse>>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentSupporters")]
    public async Task<IActionResult> GetAmendmentSupporters(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        [FromQuery] PaginationQuerySpec<object> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var supporters = await amendmentsApiClient
            .GetAmendmentSupportersAsync(amendmentId, query, cancellationToken);

        return Ok(supporters);
    }

    /// <summary>
    /// Gets opponents for a specific amendment with pagination support.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to get opponents for.</param>
    /// <param name="query">The pagination query specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of opponents for the specified amendment.</returns>
    [HttpGet("{amendmentId}/opponents")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AmendmentOpponentResponse>>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentOpponents")]
    public async Task<IActionResult> GetAmendmentOpponents(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        [FromQuery] PaginationQuerySpec<object> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(amendmentId);
        ArgumentNullException.ThrowIfNull(query);

        var opponents = await amendmentsApiClient
            .GetAmendmentOpponentsAsync(amendmentId, query, cancellationToken);

        return Ok(opponents);
    }

    /// <summary>
    /// Gets support trend data for a specific amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to get trend data for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Support trend data for the specified amendment.</returns>
    [HttpGet("{amendmentId}/trend")]
    [ProducesResponseType(typeof(ApiResponse<AmendmentTrendResponse>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getAmendmentTrend")]
    public async Task<IActionResult> GetAmendmentTrend(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        ArgumentNullException.ThrowIfNull(amendmentId);

        var trend = await amendmentsApiClient
            .GetAmendmentTrendAsync(amendmentId, cancellationToken);

        return Ok(trend);
    }

    /// <summary>
    /// Proposes a new amendment to an existing idea.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea to amend.</param>
    /// <param name="request">The request containing amendment details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Created response with the proposed amendment details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProposeAmendmentResponse>), 201)]
    [EndpointName("proposeAmendment")]
    public async Task<IActionResult> ProposeAmendment(
        [FromRoute] string ideaId,
        [FromBody] ProposeAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await amendmentsApiClient
            .ProposeAmendmentAsync(ideaId, request, cancellationToken);

        return CreatedAtAction(nameof(GetAmendments), new { ideaId }, result);
    }

    /// <summary>
    /// Adds support for an amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to support.</param>
    /// <param name="request">The request containing the reason for supporting the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("{amendmentId}/support")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("supportAmendment")]
    public async Task<IActionResult> SupportAmendment(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        [FromBody] SupportAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        var response = await amendmentsApiClient
            .SupportAmendmentAsync(amendmentId, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Adds opposition to an amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to oppose.</param>
    /// <param name="request">The request containing the reason for opposing the amendment.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("{amendmentId}/oppose")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("opposeAmendment")]
    public async Task<IActionResult> OpposeAmendment(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        [FromBody] OpposeAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        var response = await amendmentsApiClient
            .OpposeAmendmentAsync(amendmentId, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Withdraws support for an amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to withdraw support from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("{amendmentId}/withdraw-support")]
    [ProducesResponseType(200)]
    [EndpointName("withdrawSupport")]
    public async Task<IActionResult> WithdrawSupport(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        var result = await amendmentsApiClient
            .WithdrawSupportAsync(amendmentId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Withdraws opposition to an amendment.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea that contains the amendment.</param>
    /// <param name="amendmentId">The unique identifier of the amendment to withdraw opposition from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated support and opposition counts for the amendment.</returns>
    [HttpPost("{amendmentId}/withdraw-opposition")]
    [ProducesResponseType(typeof(AmendmentVoteResponse), 200)]
    [EndpointName("withdrawOpposition")]
    public async Task<IActionResult> WithdrawOpposition(
        [FromRoute] string ideaId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        var response = await amendmentsApiClient
            .WithdrawOppositionAsync(amendmentId, cancellationToken);
        return Ok(response);
    }
}
