using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for retrieving emerging patterns identified by the clustering pipeline.
/// </summary>
/// <param name="messageBus">The message bus for dispatching queries.</param>
[ApiController]
[Route("api/emerging-patterns")]
public sealed class EmergingPatternsController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Returns all currently identified emerging signal patterns.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A feed of emerging patterns ordered by cluster label.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<EmergingPatternsResponse>), 200)]
    [EndpointName("getEmergingPatterns")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEmergingPatterns(CancellationToken cancellationToken)
    {
        var patterns = await messageBus
            .InvokeAsync<IReadOnlyList<EmergingPatternView>>(new GetEmergingPatterns(), cancellationToken)
            .ConfigureAwait(false);

        var response = new EmergingPatternsResponse
        {
            Patterns = patterns
                .Select(p => new EmergingPatternResponse
                {
                    ClusterLabel = p.ClusterLabel,
                    SignalIds = p.SignalIds,
                    SignalCount = p.SignalCount,
                    LastUpdatedAt = p.LastUpdatedAt,
                    Keywords = p.Keywords,
                })
                .ToList(),
        };

        return Ok(response);
    }
}
