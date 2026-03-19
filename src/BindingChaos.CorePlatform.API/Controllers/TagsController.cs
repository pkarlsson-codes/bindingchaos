using BindingChaos.CorePlatform.Contracts.Responses.Tags;
using BindingChaos.Infrastructure.API;
using BindingChaos.Tagging.Application.Queries;
using BindingChaos.Tagging.Application.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing global tags.
/// </summary>
/// <param name="messageBus">The message bus instance used for publishing events or messages.</param>
[ApiController]
[Route("api/tags")]
public sealed class TagsController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Retrieves a list of globally popular tags.
    /// </summary>
    /// <param name="limit">The maximum number of tags to include in the response. Must be a positive integer.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An array of popular tags.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<TagResponse[]>), 200)]
    [EndpointName("getTags")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTags([FromQuery] int limit, CancellationToken cancellationToken)
    {
        var query = new GetTags(limit);
        var result = await messageBus.InvokeAsync<TagUsageView[]>(query, cancellationToken).ConfigureAwait(false);
        var response = result.Select(x => new TagResponse(x.Id, x.Slug, x.UsageCount)).ToArray();

        return Ok(response);
    }
}
