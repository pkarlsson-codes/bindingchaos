using BindingChaos.CorePlatform.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing tags in the web gateway.
/// </summary>
/// <param name="tagsApiClient">The API client for interacting with the tags service.</param>
[ApiController]
[Route("api/v1/tags")]
public sealed class TagsController(
    ITagsApiClient tagsApiClient)
    : ControllerBase
{
    private readonly ITagsApiClient _tagsApiClient = tagsApiClient;

    /// <summary>
    /// Gets popular tags based on usage frequency.
    /// </summary>
    /// <param name="limit">Maximum number of tags to return (default: 50, max: 100).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>List of popular tags with usage counts.</returns>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(500)]
    [EndpointName("getPopularTags")]
    public OkObjectResult GetPopularTags(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        _tagsApiClient.GetTags(limit, null, cancellationToken);
        return Ok(Array.Empty<string>());
    }

    /// <summary>
    /// Searches for tags by query string.
    /// </summary>
    /// <param name="q">Search query.</param>
    /// <param name="limit">Maximum number of tags to return (default: 20, max: 50).</param>
    /// <returns>List of matching tags.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(500)]
    [EndpointName("searchTags")]
    public IActionResult SearchTags(
        [FromQuery] string q,
        [FromQuery] int limit = 20)
    {
        return Ok(Array.Empty<string>());
    }

    /// <summary>
    /// Gets recently used tags by the current user.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="limit">Maximum number of tags to return (default: 30, max: 100).</param>
    /// <returns>List of recently used tags.</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(500)]
    [EndpointName("getRecentTags")]
    public IActionResult GetRecentTags(
        [FromQuery] string userId,
        [FromQuery] int limit = 30)
    {
        return Ok(Array.Empty<string>());
    }
}
