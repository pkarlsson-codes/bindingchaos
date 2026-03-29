using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing ideas in the web gateway.
/// </summary>
/// <param name="ideasApiClient">Client for interacting with the Ideas API.</param>
[ApiController]
[Route("api/v1/ideas")]
public sealed class IdeasController(IIdeasApiClient ideasApiClient) : BaseApiController
{
    /// <summary>
    /// Gets all ideas with optional filtering and pagination.
    /// </summary>
    /// <param name="query">Pagination and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Paginated list of ideas with metadata.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IdeasFeedViewModel>), 200)]
    [EndpointName("getIdeas")]
    public async Task<OkObjectResult> GetIdeas(
        [FromQuery] PaginationQuerySpec<IdeasQueryFilter> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var result = await ideasApiClient
            .GetIdeasAsync(query.Normalize(), cancellationToken);

        var response = new IdeasFeedViewModel
        {
            Ideas = result,
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new idea.
    /// </summary>
    /// <param name="request">The idea creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created idea.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("authorIdea")]
    public async Task<IActionResult> AuthorIdea(
        [FromBody] AuthorIdeaRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var domainRequest = new CorePlatform.Contracts.Requests.DraftIdeaRequest(
            request.Title,
            request.Description);

        var result = await ideasApiClient
            .AuthorIdeaAsync(domainRequest, cancellationToken);

        return CreatedAtAction(nameof(GetIdeaDetails), new { ideaId = result }, result);
    }

    /// <summary>
    /// Gets detailed information about a specific idea.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Detailed idea information.</returns>
    [HttpGet("{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<IdeaDetailViewModel>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getIdea")]
    public async Task<IActionResult> GetIdeaDetails(string ideaId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideaId);

        var result = await ideasApiClient
            .GetIdeaAsync(ideaId, cancellationToken);

        var viewModel = new IdeaDetailViewModel
        {
            Idea = result,
        };

        return Ok(viewModel);
    }
}
