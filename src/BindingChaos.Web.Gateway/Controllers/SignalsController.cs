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
    public async Task<OkObjectResult> GetSignals([FromQuery] PaginationQuerySpec<SignalsQueryFilter> query, CancellationToken cancellationToken)
    {
        query = (query ?? new PaginationQuerySpec<SignalsQueryFilter>()).Normalize();

        var signalsTask = _signalsApiClient.GetSignals(query, cancellationToken);
        var tagsTask = _tagsApiClient.GetTags(20, null, cancellationToken);

        await Task.WhenAll(signalsTask, tagsTask).ConfigureAwait(false);

        var signalsResult = await signalsTask.ConfigureAwait(false);
        var tagsResult = await tagsTask.ConfigureAwait(false);

        var viewmodel = new SignalsFeedViewModel
        {
            Signals = signalsResult.MapItems(s => new SignalViewModel
            {
                Id = s.SignalId,
                AuthorPseudonym = s.AuthorId ?? "Anonymous",
                Title = s.Title,
                Description = s.Description,
                Tags = s.Tags,
                AmplificationCount = s.AmplificationCount,
                CreatedAt = s.CapturedAt.ToString("O") ?? string.Empty,
                IsAmplifiedByCurrentUser = s.IsAmplifiedByCurrentUser,
                IsOriginator = s.IsOriginator,
                FirstAttachmentThumbnail = GetFirstAttachmentThumbnailUrl(s.Attachments),
                AttachmentCount = s.Attachments?.Length ?? 0,
            }),
            AvailableTags = [.. tagsResult.Select(t => t.Label)],
        };

        return Ok(viewmodel);
    }

    /// <summary>
    /// Creates a new signal.
    /// </summary>
    /// <param name="request">The signal creation request.</param>
    /// <returns>The created signal.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("captureSignal")]
    [AllowAnonymous]
    public async Task<IActionResult> CaptureSignal([FromBody] CaptureSignalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _signalsApiClient.CaptureSignal(request).ConfigureAwait(false);

        return Created(string.Empty, result);
    }

    /// <summary>
    /// Gets detailed information about a specific signal including amplification history.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal.</param>
    /// <returns>Detailed signal information with amplification history.</returns>
    [HttpGet("{signalId}")]
    [ProducesResponseType(typeof(ApiResponse<SignalDetailViewModel>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("getSignalDetails")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignalDetails([FromRoute(Name = "signalId")] string signalId)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var signal = await _signalsApiClient.GetSignal(signalId).ConfigureAwait(false);

        var viewModel = new SignalDetailViewModel
        {
            Id = signal.SignalId,
            Title = signal.Title,
            Description = signal.Description,
            Tags = [],
            AuthorPseudonym = signal.OriginatorPseudonym,
            CreatedAt = signal.CapturedAt.ToString("O", CultureInfo.InvariantCulture),
            LastAmplifiedAt = signal.Amplifications.DefaultIfEmpty().Max(a => a?.AmplifiedAt)?.ToString("O", CultureInfo.InvariantCulture) ?? null,
            AmplifyCount = signal.Amplifications.Count,
            Amplifications = [..signal.Amplifications.Select(a => new AmplificationViewModel
                {
                    Id = a.AmplificationId,
                    AmplifierPseudonym = a.AmplifierPseudonym,
                    AmplifiedAt = a.AmplifiedAt.ToString("O", CultureInfo.InvariantCulture),
                })],
            IsAmplifiedByCurrentUser = signal.IsAmplifiedByCurrentUser,
            IsOriginator = signal.IsOriginator,
            Attachments = [..signal.Attachments.Select(a => new AttachmentDetailViewModel
                {
                    DocumentId = a.DocumentId,
                    Caption = a.Caption,
                    ThumbnailUrl = $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{a.DocumentId}/thumbnail",
                    DisplayUrl = $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{a.DocumentId}/display",
                })],
            SuggestedActions = [..signal.SuggestedActions.Select(a => new SuggestedActionViewModel
                {
                    Id = a.ActionId,
                    ActionType = a.ActionType,
                    PhoneNumber = a.PhoneNumber,
                    Url = a.Url,
                    Details = a.Details,
                    SuggestedByPseudonym = a.SuggestedByPseudonym,
                    SuggestedAt = a.SuggestedAt.ToString("O", CultureInfo.InvariantCulture),
                })],
        };

        return Ok(viewModel);
    }

    /// <summary>
    /// Amplifies a signal by a participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to amplify.</param>
    /// <param name="request">The amplification request.</param>
    /// <returns>The amplification response with updated count.</returns>
    [HttpPost("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<AmplifySignalResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("amplifySignal")]
    public async Task<IActionResult> AmplifySignal(string signalId, [FromBody] AmplifySignalRequest request)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(request);

        var response = await _signalsApiClient.AmplifySignal(signalId, request)
            .ConfigureAwait(false);

        return Ok(response);
    }

    /// <summary>
    /// Removes a user's amplification from a signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to deamplify.</param>
    /// <returns>The deamplification response with updated count.</returns>
    [HttpDelete("{signalId}/amplifications")]
    [ProducesResponseType(typeof(ApiResponse<DeamplifySignalResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [EndpointName("deamplifySignal")]
    public async Task<IActionResult> DeamplifySignal(string signalId)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var response = await _signalsApiClient.DeamplifySignal(signalId)
            .ConfigureAwait(false);

        return Ok(response);
    }

    /// <summary>
    /// Suggests an action on a signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal.</param>
    /// <param name="request">The action suggestion request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A 201 Created response.</returns>
    [HttpPost("{signalId}/suggested-actions")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [EndpointName("suggestAction")]
    public async Task<IActionResult> SuggestAction(string signalId, [FromBody] SuggestActionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signalId);
        ArgumentNullException.ThrowIfNull(request);

        await _signalsApiClient.SuggestAction(signalId, request, cancellationToken).ConfigureAwait(false);

        return Created(string.Empty, null);
    }

    /// <summary>
    /// Gets amplification trend data for a specific signal.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal.</param>
    /// <returns>Signal amplification trend data.</returns>
    [HttpGet("{signalId}/amplification-trend")]
    [ProducesResponseType(typeof(ApiResponse<SignalAmplificationTrendResponse>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getSignalAmplificationTrend")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSignalAmplificationTrend([FromRoute] string signalId)
    {
        ArgumentNullException.ThrowIfNull(signalId);

        var trend = await _signalsApiClient.GetSignalAmplificationTrendAsync(signalId).ConfigureAwait(false);
        return Ok(trend);
    }

    /// <summary>
    /// Gets the thumbnail URL for the first image attachment, if any exist.
    /// </summary>
    /// <param name="attachments">The list of attachments.</param>
    /// <returns>The thumbnail URL or null if no image attachments exist.</returns>
    private string? GetFirstAttachmentThumbnailUrl(AttachmentResponse[]? attachments)
    {
        if (attachments == null || attachments.Length == 0)
        {
            return null;
        }

        var firstAttachment = attachments[0];
        if (string.IsNullOrEmpty(firstAttachment.DocumentId))
        {
            return null;
        }

        return $"{_gatewayBaseUrl.TrimEnd('/')}/api/v1/documents/{firstAttachment.DocumentId}/thumbnail";
    }
}
