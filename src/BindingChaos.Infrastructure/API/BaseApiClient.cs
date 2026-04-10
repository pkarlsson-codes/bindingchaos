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
    protected BaseApiClient(
        HttpClient httpClient,
        ILogger logger,
        ResiliencePipeline<HttpResponseMessage>? pipeline = null)
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
    protected async Task<T> GetAsync<T>(
        string endpoint,
        CancellationToken cancellationToken)
        where T : class
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(
                    new ResiliencePropertyKey<string>("uri"),
                    endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .GetAsync(endpoint, ctx.CancellationToken)
                            .ConfigureAwait(false), context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
                }

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
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .PostAsJsonAsync(endpoint, request, JsonOptions, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
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

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .PutAsJsonAsync(endpoint, request, JsonOptions, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
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
    /// Performs a DELETE request.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient.DeleteAsync(endpoint, ctx.CancellationToken)
                        .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
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
    /// Performs a POST request with a request body but no response body (e.g., 204 No Content).
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="request">The request data to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .PostAsJsonAsync(endpoint, request, JsonOptions, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
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
    /// Performs a POST request with no request body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task PostAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient.PostAsync(endpoint, null, ctx.CancellationToken).ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
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
    /// Performs a POST request with no request body and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    protected async Task<TResponse> PostAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient.PostAsync(endpoint, null, ctx.CancellationToken).ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
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
    /// Performs a PUT request with no request body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task PutAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient.PutAsync(endpoint, null, ctx.CancellationToken).ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
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
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .DeleteAsync(endpoint, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);
                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
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
    protected async Task<TResponse> PostFormAsync<TResponse>(
        string endpoint,
        HttpContent formData,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        try
        {
            ArgumentNullException.ThrowIfNull(formData);

            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .PostAsync(endpoint, formData, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TResponse>>(JsonOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (apiResponse?.Data == null)
                {
                    Logs.ApiResponseDataNull(Logger, endpoint);
                    throw new InvalidOperationException($"API response data is null for endpoint: {endpoint}");
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
    /// Performs a GET request and returns the raw response stream.
    /// </summary>
    /// <param name="endpoint">The API endpoint to call.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response content stream.</returns>
    protected async Task<Stream> GetStreamAsync(
        string endpoint,
        CancellationToken cancellationToken)
    {
        try
        {
            var context = ResilienceContextPool.Shared.Get(cancellationToken);
            try
            {
                context.Properties.Set(new ResiliencePropertyKey<string>("uri"), endpoint);
                var response = await Pipeline
                    .ExecuteAsync(async ctx =>
                        await HttpClient
                            .GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, ctx.CancellationToken)
                            .ConfigureAwait(false),
                        context)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                return await response.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);
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
                    .HandleResult(response =>
                        !response.IsSuccessStatusCode &&
                        (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                        (int)response.StatusCode >= 500)),
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "API response data is null for endpoint: {Endpoint}")]
        internal static partial void ApiResponseDataNull(ILogger logger, string endpoint);

        [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "HTTP request failed for endpoint: {Endpoint}")]
        internal static partial void HttpRequestFailed(ILogger logger, string endpoint, Exception ex);

        [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = "Failed to deserialize response for endpoint: {Endpoint}")]
        internal static partial void DeserializationFailed(ILogger logger, string endpoint, Exception ex);
    }
}
