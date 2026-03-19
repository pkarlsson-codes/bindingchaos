using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Application.Queries;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Pseudonymity.Application.Services;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Application.Queries;
using BindingChaos.SignalAwareness.Application.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing ideas.
/// </summary>
/// <param name="messageBus">The message bus instance used for publishing events or messages.</param>
/// <param name="pseudonymService">The pseudonym service for resolving participant identities.</param>
[ApiController]
[Route("api/ideas")]
public sealed class IdeasController(IMessageBus messageBus, IPseudonymService pseudonymService) : BaseApiController
{
    /// <summary>
    /// Creates a new idea.
    /// </summary>
    /// <param name="request">The idea creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created idea.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("authorIdea")]
    public async Task<IActionResult> AuthorIdea([FromBody] AuthorIdeaRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var createIdeaCommand = new AuthorIdea(
            request.Title,
            request.Body,
            participantId,
            SocietyId.Create(request.SocietyId),
            request.SourceSignalIds,
            request.Tags);

        var ideaId = await messageBus.InvokeAsync<IdeaId>(createIdeaCommand, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetIdea), new { ideaId = ideaId.Value }, ideaId.Value);
    }

    /// <summary>
    /// Gets a specific idea by its ID.
    /// </summary>
    /// <param name="ideaId">The ID of the idea to retrieve.</param>
    /// <returns>A response containing the idea details.</returns>
    [HttpGet("{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<IdeaResponse>), 200)]
    [EndpointName("getIdea")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIdea([FromRoute] string ideaId)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        var query = new GetIdea(IdeaId.Create(ideaId));
        var idea = await messageBus.InvokeAsync<IdeaView?>(query).ConfigureAwait(false);
        if (idea == null)
        {
            return NotFound($"Idea with ID {ideaId} not found.");
        }

        var signals = await messageBus.InvokeAsync<IReadOnlyList<SignalTitle>>(new GetSignalTitlesByIds([.. idea.SignalReferenceIds])).ConfigureAwait(false);

        var sourceSignals = signals.Select(s => new IdeaSourceSignal(s.SignalId, s.Title)).ToList();
        var authorPseudonym = pseudonymService.Generate(IdeaId.Create(idea.Id), idea.AuthorId);

        return Ok(IdeaMapper.ToIdeaResponse(idea, sourceSignals, authorPseudonym));
    }

    /// <summary>
    /// Gets all ideas.
    /// </summary>
    /// <param name="request">The request parameters for filtering ideas.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of ideas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<IdeaListItemResponse>>), 200)]
    [EndpointName("getIdeas")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIdeas([FromQuery] PaginationQuerySpec<IdeasQueryFilter> request, CancellationToken cancellationToken)
    {
        var query = new GetIdeas(request);
        var result = await messageBus.InvokeAsync<PaginatedResponse<IdeasListItemView>>(query, cancellationToken).ConfigureAwait(false);

        return Ok(result.MapItems(IdeaMapper.ToIdeaListItemResponse));
    }
}
