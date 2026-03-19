namespace BindingChaos.CorePlatform.Contracts.Responses;
/// <summary>
/// Response model for signal creation.
/// </summary>
/// <param name="SignalId">The ID of the created signal.</param>
public record CaptureSignalResponse(string SignalId);
