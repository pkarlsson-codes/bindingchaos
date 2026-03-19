namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Represents the title of a signal, including its unique identifier and descriptive title.
/// </summary>
/// <param name="SignalId">The unique identifier of the signal. This value cannot be null or empty.</param>
/// <param name="Title">The descriptive title of the signal. This value cannot be null or empty.</param>
public sealed record SignalTitle(string SignalId, string Title);
