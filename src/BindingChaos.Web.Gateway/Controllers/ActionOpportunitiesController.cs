using BindingChaos.Infrastructure.API;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing action opportunities in the web gateway.
/// </summary>
[ApiController]
[Route("api/v1/action-opportunities")]
public sealed class ActionOpportunitiesController : ControllerBase
{
    /// <summary>
    /// Gets all action opportunities with optional filtering and pagination.
    /// </summary>
    /// <param name="searchTerm">Search term for title and description.</param>
    /// <param name="status">Status filter: emerging, in-progress, completed.</param>
    /// <param name="parentIdeaId">Filter by parent idea ID.</param>
    /// <param name="sortBy">Sort option: recent, updated, title, participants, progress.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page (default: 20, max: 100).</param>
    /// <returns>Paginated list of action opportunities with metadata.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ActionOpportunitiesFeedViewModel), 200)]
    [ProducesResponseType(500)]
    [EndpointName("getActionOpportunities")]
    public OkObjectResult GetActionOpportunities(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] string? parentIdeaId = null,
        [FromQuery] string? sortBy = "recent",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var paginatedActionOpportunities = new PaginatedResponse<ActionOpportunityListResponse>
        {
            Items = [],
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0,
        };

        var response = new ActionOpportunitiesFeedViewModel
        {
            ActionOpportunities = paginatedActionOpportunities,
            AvailableStatuses = [],
            AppliedFilters = new ActionOpportunitiesFilterStateViewModel
            {
                SearchTerm = searchTerm,
                Status = status ?? "all",
                ParentIdeaId = parentIdeaId,
                SortBy = sortBy ?? "recent",
            },
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new action opportunity from an idea.
    /// </summary>
    /// <param name="request">The action opportunity creation request.</param>
    /// <returns>The created action opportunity.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiEnvelope<ActionOpportunityResponse>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [EndpointName("createActionOpportunity")]
    public IActionResult CreateActionOpportunity([FromBody] CreateActionOpportunityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return CreatedAtAction(nameof(GetActionOpportunities), null!);
    }

    /// <summary>
    /// Gets detailed information about a specific action opportunity.
    /// </summary>
    /// <param name="actionOpportunityId">The unique identifier of the action opportunity.</param>
    /// <returns>Detailed action opportunity information.</returns>
    [HttpGet("{actionOpportunityId}")]
    [ProducesResponseType(typeof(ApiEnvelope<ActionOpportunityResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("getActionOpportunityDetails")]
    public IActionResult GetActionOpportunityDetails(string actionOpportunityId)
    {
        return NotFound(new { error = "Action opportunity not found" });
    }
}
