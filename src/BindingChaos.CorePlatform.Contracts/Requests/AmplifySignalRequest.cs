namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for amplifying a signal.
/// </summary>
/// <param name="Reason">The reason for amplifying the signal.</param>
/// <param name="Commentary">Optional commentary about the amplification.</param>
public record AmplifySignalRequest(string? Reason, string? Commentary);
