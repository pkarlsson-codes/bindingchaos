using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Base controller that provides standardized API response wrapping.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns a 200 OK response with data wrapped in ApiResponse.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>An <see cref="OkObjectResult"/> containing the wrapped response.</returns>
    protected OkObjectResult Ok<T>(T data)
    {
        var response = ApiResponse.CreateSuccess(data);
        return base.Ok(response);
    }

    /// <summary>
    /// Returns a 201 Created response with data wrapped in ApiResponse.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    /// <param name="uri">The URI of the newly created resource.</param>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>A <see cref="CreatedResult"/> containing the wrapped response.</returns>
    protected CreatedResult Created<T>(Uri uri, T data)
    {
        var response = ApiResponse.CreateSuccess(data);
        return base.Created(uri, response);
    }

    /// <summary>
    /// Returns a 201 Created response with data wrapped in ApiResponse.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    /// <param name="uri">The URI of the newly created resource.</param>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>A <see cref="CreatedResult"/> containing the wrapped response.</returns>
    protected CreatedResult Created<T>(string uri, T data)
    {
        var response = ApiResponse.CreateSuccess(data);
        return base.Created(uri, response);
    }

    /// <summary>
    /// Returns a 201 Created response with data wrapped in ApiResponse.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    /// <param name="actionName">The action name to use for generating the Location header.</param>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> containing the wrapped response.</returns>
    protected CreatedAtActionResult CreatedAtAction<T>(string actionName, T data)
    {
        var response = ApiResponse.CreateSuccess(data);
        return base.CreatedAtAction(actionName, response);
    }

    /// <summary>
    /// Returns a 201 Created response with data wrapped in ApiResponse and a Location header generated from the specified action and route values.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    /// <param name="actionName">The action name to use for generating the Location header.</param>
    /// <param name="routeValues">The route values to use for generating the Location header.</param>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> containing the wrapped response.</returns>
    protected CreatedAtActionResult CreatedAtAction<T>(string actionName, object? routeValues, T data)
    {
        var response = ApiResponse.CreateSuccess(data);
        return base.CreatedAtAction(actionName, routeValues, response);
    }
}
