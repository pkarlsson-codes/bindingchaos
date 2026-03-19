using System.Text;
using System.Text.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// HTTP client implementation for the Discourse API.
/// </summary>
internal sealed class DiscourseApiClient : IDiscourseApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscourseApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public DiscourseApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }

    /// <inheritdoc />
    public async Task<GetContributionsResponse> GetContributionsByThreadIdAsync(
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

        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<GetContributionsResponse>>(content, _jsonOptions);

        if (apiResponse?.Data == null)
        {
            throw new InvalidOperationException("Invalid response format from Discourse API");
        }

        return apiResponse.Data;
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

        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<GetContributionRepliesResponse>>(content, _jsonOptions);

        if (apiResponse?.Data == null)
        {
            throw new InvalidOperationException("Invalid response format from Discourse API");
        }

        return apiResponse.Data;
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

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<PostContributionResponse>>(responseContent, _jsonOptions);

        if (apiResponse?.Data == null)
        {
            throw new InvalidOperationException("Invalid response format from Discourse API");
        }

        return apiResponse.Data;
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

        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<GetContributionsResponse>>(content, _jsonOptions);

        if (apiResponse?.Data == null)
        {
            throw new InvalidOperationException("Invalid response format from Discourse API");
        }

        return apiResponse.Data;
    }
}