namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Static factory methods for creating API responses.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <typeparam name="T">The type of data in the response.</typeparam>
    /// <returns>A new successful API response.</returns>
    public static ApiResponse<T> CreateSuccess<T>(T data)
    {
        return new ApiResponse<T>
        {
            Data = data,
        };
    }
}

/// <summary>
/// Standardized API response envelope as defined in ADR-00016.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; set; }
}