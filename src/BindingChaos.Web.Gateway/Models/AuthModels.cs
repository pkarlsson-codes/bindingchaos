namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request payload to authenticate a user with the gateway.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the username or login identifier for the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response returned after an authentication attempt.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the login attempt was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error message when <see cref="Success"/> is false; otherwise null.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user information when successful.
    /// </summary>
    public UserInfo? User { get; set; }

    /// <summary>
    /// Gets or sets the server-issued session identifier when successful.
    /// </summary>
    public string? SessionId { get; set; }
}

/// <summary>
/// Basic user profile information used by the gateway.
/// </summary>
public sealed class UserInfo
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's canonical username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's pseudonym as displayed in context.
    /// </summary>
    public string Pseudonym { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Response returned after a logout attempt.
/// </summary>
public sealed class LogoutResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the logout operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error message when <see cref="Success"/> is false; otherwise null.
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Response returned when querying the current authenticated user.
/// </summary>
public sealed class GetCurrentUserResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error message when <see cref="Success"/> is false; otherwise null.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the currently authenticated user information when available.
    /// </summary>
    public UserInfo? User { get; set; }
}