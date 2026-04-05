using System.Text.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// HTTP client implementation for the Discourse API.
/// </summary>
/// <param name="httpClient">The HTTP client.</param>
/// <param name="logger">The logger instance used to log diagnostic messages.</param>
internal sealed class DiscourseApiClient(
    HttpClient httpClient,
    ILogger<DiscourseApiClient> logger)
    : BaseApiClient(httpClient, logger), IDiscourseApiClient
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    /// <inheritdoc />
    public Task<GetContributionsResponse> GetContributionsByThreadIdAsync(
        string threadId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId);

        var queryParams = new List<string>
        {
            $"limit={limit}",
            $"direction={direction}",
        };

        if (!string.IsNullOrEmpty(cursor))
        {
            queryParams.Add($"cursor={Uri.EscapeDataString(cursor)}");
        }

        var queryString = string.Join("&", queryParams);
        var requestUri = $"api/discourse/threads/{Uri.EscapeDataString(threadId)}/contributions?{queryString}";

        return GetAsync<GetContributionsResponse>(requestUri, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GetContributionRepliesResponse> GetContributionRepliesAsync(
        string contributionId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contributionId);

        var queryParams = new List<string>
        {
            $"limit={limit}",
            $"direction={direction}",
        };

        if (!string.IsNullOrEmpty(cursor))
        {
            queryParams.Add($"cursor={Uri.EscapeDataString(cursor)}");
        }

        var queryString = string.Join("&", queryParams);
        var requestUri = $"api/discourse/contributions/{Uri.EscapeDataString(contributionId)}/replies?{queryString}";

        var response = await GetAsync<GetContributionRepliesResponse>(requestUri, cancellationToken)
            .ConfigureAwait(false);

        return response;
    }

    /// <inheritdoc />
    public async Task<PostContributionResponse> PostContributionToThreadAsync(
        string threadId,
        PostContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId);
        ArgumentNullException.ThrowIfNull(request);

        var requestUri = $"api/discourse/threads/{Uri.EscapeDataString(threadId)}/contributions";

        var response = await PostAsync<PostContributionRequest, PostContributionResponse>(requestUri, request, cancellationToken)
            .ConfigureAwait(false);

        return response;
    }

    /// <inheritdoc />
    public async Task<GetContributionsResponse> GetContributionsByEntityAsync(
        string entityType,
        string entityId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId);

        var queryParams = new List<string>
        {
            $"limit={limit}",
            $"direction={direction}",
        };

        if (!string.IsNullOrEmpty(cursor))
        {
            queryParams.Add($"cursor={Uri.EscapeDataString(cursor)}");
        }

        var queryString = string.Join("&", queryParams);
        var requestUri = $"api/discourse/threads/by-entity/{Uri.EscapeDataString(entityType)}/{Uri.EscapeDataString(entityId)}/contributions?{queryString}";

        var result = await GetAsync<GetContributionsResponse>(requestUri, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }
}