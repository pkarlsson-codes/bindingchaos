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
/// <param name="societiesApiClient">Client for interacting with the Societies API.</param>
[ApiController]
[Route("api/v1/ideas")]
public sealed class IdeasController(
    IIdeasApiClient ideasApiClient,
    ISocietiesApiClient societiesApiClient)
    : BaseApiController
{
    /// <summary>
    /// Gets all ideas with optional filtering and pagination.
    /// When <paramref name="filterToMySocieties"/> is <c>true</c>, results are scoped to the
    /// authenticated participant's active society memberships. Anonymous users receive an empty list.
    /// </summary>
    /// <param name="query">Pagination and filter parameters.</param>
    /// <param name="filterToMySocieties">When true, restricts results to the current user's societies.</param>
    /// <returns>Paginated list of ideas with metadata.</returns>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IdeasFeedViewModel>), 200)]
    [EndpointName("getIdeas")]
    public async Task<OkObjectResult> GetIdeas(
        [FromQuery] PaginationQuerySpec<IdeasQueryFilter> query,
        [FromQuery] bool filterToMySocieties = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var normalizedQuery = query.Normalize();

        if (filterToMySocieties)
        {
            var societyIds = await societiesApiClient
                .GetMySocietyIdsAsync(cancellationToken);
            normalizedQuery = new PaginationQuerySpec<IdeasQueryFilter>
            {
                Page = normalizedQuery.Page,
                Filter = (normalizedQuery.Filter ?? new IdeasQueryFilter()) with { SocietyIds = societyIds },
                SortDescriptors = normalizedQuery.SortDescriptors,
            };
        }

        var result = await ideasApiClient
            .GetIdeasAsync(normalizedQuery, cancellationToken);

        var response = new IdeasFeedViewModel
        {
            Ideas = result,
            AvailableTags = [.. result.Items.SelectMany(x => x.Tags)],
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

        var domainRequest = new CorePlatform.Contracts.Requests.AuthorIdeaRequest(
            request.Title,
            request.Description,
            request.SocietyId,
            [.. request.SourceSignalIds],
            [.. request.Tags]);

        var result = await ideasApiClient
            .AuthorIdeaAsync(domainRequest, cancellationToken);

        return CreatedAtAction(nameof(GetIdeaDetails), new { ideaId = result }, result);
    }

    /// <summary>
    /// Gets detailed information about a specific idea including amendments and lineage.
    /// </summary>
    /// <param name="ideaId">The unique identifier of the idea.</param>
    /// <returns>Detailed idea information with amendments and lineage.</returns>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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
