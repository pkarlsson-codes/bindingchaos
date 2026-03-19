using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for querying the action type catalog.
/// </summary>
[ApiController]
[Route("api/action-types")]
public sealed class ActionTypesController : BaseApiController
{
    /// <summary>
    /// Returns all available action types.
    /// Display labels are the responsibility of the presentation layer;
    /// use the <c>Name</c> field as the <c>ActionType</c> value when submitting a suggestion.
    /// </summary>
    /// <returns>The ordered list of available action types.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ActionTypeResponse>), 200)]
    [EndpointName("getActionTypes")]
    public IActionResult GetActionTypes()
    {
        var response = ActionType.GetAll()
            .OrderBy(a => a.Value)
            .Select(a => new ActionTypeResponse(a.Value, a.DisplayName));

        return Ok(response);
    }
}
