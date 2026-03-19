using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for querying the action type catalog.
/// </summary>
[ApiController]
[Route("api/v1/action-types")]
public sealed class ActionTypesController : BaseApiController
{
    private readonly ISignalsApiClient _signalsApiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionTypesController"/> class.
    /// </summary>
    /// <param name="signalsApiClient">The API client for interacting with the signals service.</param>
    public ActionTypesController(ISignalsApiClient signalsApiClient)
    {
        _signalsApiClient = signalsApiClient ?? throw new ArgumentNullException(nameof(signalsApiClient));
    }

    /// <summary>
    /// Returns all available action types.
    /// Display labels are the responsibility of the presentation layer;
    /// use the <c>Name</c> field as the <c>ActionType</c> value when submitting a suggestion.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ordered list of available action types.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ActionTypeResponse>), 200)]
    [EndpointName("getActionTypes")]
    public async Task<IActionResult> GetActionTypes(CancellationToken cancellationToken)
    {
        var types = await _signalsApiClient.GetActionTypes(cancellationToken).ConfigureAwait(false);
        return Ok(types);
    }
}
