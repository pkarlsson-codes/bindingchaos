using BindingChaos.CorePlatform.Clients;
using BindingChaos.Infrastructure.API;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for retrieving emerging signal patterns.
/// </summary>
/// <param name="emergingPatternsApiClient">The API client for interacting with the emerging patterns service.</param>
[ApiController]
[Route("api/v1/emerging-patterns")]
public sealed class EmergingPatternsController(IEmergingPatternsApiClient emergingPatternsApiClient) : BaseApiController
{
    /// <summary>
    /// Returns all currently identified emerging signal patterns.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A feed of emerging patterns ordered by cluster label.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<EmergingPatternsFeedViewModel>), 200)]
    [EndpointName("getEmergingPatterns")]
    [AllowAnonymous]
    public async Task<OkObjectResult> GetEmergingPatterns(CancellationToken cancellationToken)
    {
        var feed = await emergingPatternsApiClient
            .GetEmergingPatternsAsync(cancellationToken);

        return Ok(new EmergingPatternsFeedViewModel { Patterns = feed.Patterns });
    }
}
