using System.Globalization;
using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Web.Gateway.Configuration;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing signals in the web gateway.
/// </summary>
[ApiController]
[Route("api/v1/signals")]
public sealed class SignalsController : BaseApiController
{
    private readonly ISignalsApiClient _signalsApiClient;
    private readonly ITagsApiClient _tagsApiClient;
    private readonly string _gatewayBaseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsController"/> class.
    /// </summary>
    /// <param name="signalsApiClient">The API client for interacting with the signals service.</param>
    /// <param name="tagsApiClient">The API client for interacting with the tags service.</param>
    /// <param name="gatewayOptions">The gateway options containing the base URL.</param>
    public SignalsController(
        ISignalsApiClient signalsApiClient,
        ITagsApiClient tagsApiClient,
        IOptions<GatewayOptions> gatewayOptions)
    {
        ArgumentNullException.ThrowIfNull(gatewayOptions);
        _signalsApiClient = signalsApiClient ?? throw new ArgumentNullException(nameof(signalsApiClient));
        _tagsApiClient = tagsApiClient ?? throw new ArgumentNullException(nameof(tagsApiClient));
        _gatewayBaseUrl = gatewayOptions.Value.BaseUrl;
    }

    /// <summary>
    /// Gets all signals with optional filtering and pagination.
    /// </summary>
    /// <param name="query">QuerySpec with pagination, sorting, and filters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of filtered signals with metadata.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SignalsFeedViewModel>), 200)]
    [EndpointName("getSignals")]
    [AllowAnonymous]
    public async Task<OkObjectResult> GetSignals(
        [FromQuery] PaginationQuerySpec<SignalsQueryFilter> query,
        CancellationToken cancellationToken)
    {
        query = (query ?? new()).Normalize();

        var signalsTask = _signalsApiClient.GetSignals(query, cancellationToken);
        var tagsTask = _tagsApiClient.GetTags(20, null, cancellationToken);

        await Task.WhenAll(signalsTask, tagsTask);

        var signalsResult = signalsTask.Result;
        var tagsResult = tagsTask.Result;

        var viewmodel = new SignalsFeedViewModel
        {
            Signals = signalsResult.MapItems(s => new SignalViewModel
            {
                Id = s.SignalId,
                AuthorPseudonym = s.OriginatorPseudonym ?? "Anonymous",
                Title = s.Title,
                Description = s.Description,
                Tags = s.Tags,
                AmplificationCount = s.AmplificationCount,
                CreatedAt = s.CapturedAt.ToString("O") ?? string.Empty,
                IsAmplifiedByCurrentUser = s.IsAmplifiedByCurrentUser,
                IsOriginator = s.IsOriginator,
                FirstAttachmentThumbnail = GetFirstAttachmentThumbnailUrl(s.AttachmentIds),
                AttachmentCount = s.AttachmentIds?.Length ?? 0,
            }),
            AvailableTags = [.. tagsResult.Select(t => t.Label)],
        };

        return Ok(viewmodel);
    }

    /// <summary>
    /// Creates a new signal.
    /// </summary>
    /// <param name="request">The signal creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created signal.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("captureSignal")]
    [AllowAnonymous]
    public async Task<IActionResult> CaptureSignal(
        [FromBody] CaptureSignalRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _signalsApiClient
            .CaptureSignal(request, cancellationToken);

        return Created(string.Empty, result);
    }

    /// <summary>
    /// Gets detailed information about a specific signal including amplification history.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Detailed signal information with amplification history.</returns>
    [HttpGet("{signalId}")]
    [ProducesResponseType(typeof(ApiResponse<SignalDetailViewModel>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("getSignalDetails")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignalDetails(
        [FromRoute(Name = "signalId")] string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var signal = await _signalsApiClient
            .GetSignal(signalId, cancellationToken);

        var viewModel = new SignalDetailViewModel
        {
            Id = signal.SignalId,
            Title = signal.Title,
            Description = signal.Description,
            Tags = [],
            AuthorPseudonym = signal.OriginatorPseudonym,
            CreatedAt = signal.CapturedAt.ToString("O", CultureInfo.InvariantCulture),
            LastAmplifiedAt = signal.LastAmplifiedAt
                ?.ToString("O", CultureInfo.InvariantCulture),
            AmplifyCount = signal.Amplifications.Count,
            Amplifications = [.. signal.Amplifications
                .Select(a => new AmplificationViewModel
                {
                    AmplifierPseudonym = a.AmplifierPseudonym,
                    AmplifiedAt = a.AmplifiedAt.ToString("O", CultureInfo.InvariantCulture),
                })],
            IsAmplifiedByCurrentUser = signal.IsAmplifiedByCurrentUser,
            IsOriginator = signal.IsOriginator,
            Attachments = [.. signal.AttachmentIds.Select(id => new AttachmentDetailViewModel
                {
                    DocumentId = id,
                    ThumbnailUrl = $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{id}/thumbnail",
                    DisplayUrl = $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{id}/display",
                })],
            SuggestedActions = [],
        };

        return Ok(viewModel);
    }

    /// <summary>
    /// Amplifies a signal by the current participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to amplify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amplification response with updated count.</returns>
    [HttpPost("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<AmplifySignalResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("amplifySignal")]
    public async Task<IActionResult> AmplifySignal(
        [FromRoute] string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var response = await _signalsApiClient
            .AmplifySignal(signalId, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Removes a user's amplification from a signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to deamplify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deamplification response with updated count.</returns>
    [HttpDelete("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<DeamplifySignalResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("deamplifySignal")]
    public async Task<IActionResult> DeamplifySignal(
        string signalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var response = await _signalsApiClient
            .DeamplifySignal(signalId, cancellationToken);

        return Ok(response);
    }

    private string? GetFirstAttachmentThumbnailUrl(string[]? attachmentIds)
    {
        if (attachmentIds == null || attachmentIds.Length == 0)
        {
            return null;
        }

        var firstId = attachmentIds[0];
        if (string.IsNullOrEmpty(firstId))
        {
            return null;
        }

        return $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{firstId}/thumbnail";
    }
}
