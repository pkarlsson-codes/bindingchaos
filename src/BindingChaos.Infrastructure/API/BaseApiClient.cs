using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Base class for API clients that provides common HTTP operations and error handling.
/// </summary>
public abstract partial class BaseApiClient
{
    /// <summary>
    /// Initializes a new instance of the BaseApiClient class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API requests.</param>
    /// <param name="logger">The logger for this client.</param>
    /// <param name="pipeline">Optional resilience pipeline. If null, a default pipeline is created.</param>
    protected BaseApiClient(HttpClient httpClient, ILogger logger, ResiliencePipeline<HttpResponseMessage>? pipeline = null)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        Pipeline = pipeline ?? CreateDefaultPipeline();
    }

    /// <summary>
    /// The HTTP client used for making API requests.
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// The logger for this API client.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// JSON serialization options for request/response handling.
    /// </summary>
    protected JsonSerializerOptions JsonOptions { get; }

    /// <summary>
    /// The Polly resilience pipeline for resilient HTTP requests.
    /// </summary>
    protected ResiliencePipeline<HttpResponseMessage> Pipeline { get; }

    /// <summary>
    /// Performs a GET request and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    protected async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        where T : class
    {
        try
        {
            Logs.MakingGetRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.GetAsync(endpoint, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, cancellationToken).ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

                Logs.SuccessfullyRetrievedData(Logger, endpoint);
                return apiResponse.Data;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a POST request and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="request">The request data to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            Logs.MakingPostRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.PostAsJsonAsync(endpoint, request, JsonOptions, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken).ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

                Logs.SuccessfullyPostedData(Logger, endpoint);
                return apiResponse.Data!;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a PUT request and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="request">The request data to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            Logs.MakingPutRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.PutAsJsonAsync(endpoint, request, JsonOptions, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken).ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

                Logs.SuccessfullyUpdatedData(Logger, endpoint);
                return apiResponse.Data!;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            Logs.MakingDeleteRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.DeleteAsync(endpoint, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                Logs.SuccessfullyDeletedData(Logger, endpoint);
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a PUT request with no request body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task PutAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            Logs.MakingPutRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.PutAsync(endpoint, null, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                Logs.SuccessfullyUpdatedData(Logger, endpoint);
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Sends an HTTP DELETE request to the specified endpoint and returns the deserialized response data.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data expected from the API.</typeparam>
    /// <param name="endpoint">The URI of the endpoint to which the DELETE request is sent. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The deserialized response data of type <typeparamref name="TResponse"/> returned by the API.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the API response data is null.</exception>
    protected async Task<TResponse> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            Logs.MakingDeleteRequest(Logger, endpoint);
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.DeleteAsync(endpoint, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken).ConfigureAwait(false);
                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

                Logs.SuccessfullyDeletedData(Logger, endpoint);
                return apiResponse.Data!;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a GET request that returns a collection and handles empty responses gracefully.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data or an empty collection if null.</returns>
    protected async Task<IEnumerable<T>> GetCollectionAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        where T : class
    {
        try
        {
            Logs.MakingGetRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.GetAsync(endpoint, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<T>>>(JsonOptions, cancellationToken).ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    return Enumerable.Empty<T>();
                }

                if (Logger.IsEnabled(LogLevel.Information))
                {
                    var itemCount = apiResponse.Data is ICollection<T> collection
                        ? collection.Count
                        : apiResponse.Data.Count();
                    Logs.SuccessfullyRetrievedCollection(Logger, itemCount, endpoint);
                }

                return apiResponse.Data!;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Performs a POST request with FormData and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="formData">The FormData content to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    protected async Task<TResponse> PostFormAsync<TResponse>(string endpoint, HttpContent formData, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(formData);

            Logs.MakingPostRequest(Logger, endpoint);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline.ExecuteAsync(async ctx =>
                    await HttpClient.PostAsync(endpoint, formData, ctx.CancellationToken).ConfigureAwait(false), context).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken).ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

                Logs.SuccessfullyPostedData(Logger, endpoint);
                return apiResponse.Data!;
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (HttpRequestException ex)
        {
            Logs.HttpRequestFailed(Logger, endpoint, ex);
            throw;
        }
        catch (JsonException ex)
        {
            Logs.DeserializationFailed(Logger, endpoint, ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a default resilience pipeline with retry and timeout for resilient HTTP requests.
    /// </summary>
    /// <returns>A default resilience pipeline.</returns>
    private static ResiliencePipeline<HttpResponseMessage> CreateDefaultPipeline()
    {
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .HandleResult(response => !response.IsSuccessStatusCode &&
                                             (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                                              (int)response.StatusCode >= 500)),
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Making GET request to endpoint: {Endpoint}")]
        internal static partial void MakingGetRequest(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Making POST request to endpoint: {Endpoint}")]
        internal static partial void MakingPostRequest(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Making PUT request to endpoint: {Endpoint}")]
        internal static partial void MakingPutRequest(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Making DELETE request to endpoint: {Endpoint}")]
        internal static partial void MakingDeleteRequest(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Successfully retrieved data from endpoint: {Endpoint}")]
        internal static partial void SuccessfullyRetrievedData(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 6, Level = LogLevel.Debug, Message = "Successfully posted data to endpoint: {Endpoint}")]
        internal static partial void SuccessfullyPostedData(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 7, Level = LogLevel.Debug, Message = "Successfully updated data at endpoint: {Endpoint}")]
        internal static partial void SuccessfullyUpdatedData(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "Successfully deleted data at endpoint: {Endpoint}")]
        internal static partial void SuccessfullyDeletedData(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 9, Level = LogLevel.Debug, Message = "Successfully retrieved {Count} items from endpoint: {Endpoint}")]
        internal static partial void SuccessfullyRetrievedCollection(ILogger logger, int count, string endpoint);

        [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "API response data is null for endpoint: {Endpoint}")]
        internal static partial void ApiResponseDataNull(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "HTTP request failed for endpoint: {Endpoint}")]
        internal static partial void HttpRequestFailed(ILogger logger, string endpoint, Exception ex);

        [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = "Failed to deserialize response for endpoint: {Endpoint}")]
        internal static partial void DeserializationFailed(ILogger logger, string endpoint, Exception ex);
    }
}
