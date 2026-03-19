namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for adding evidence to a signal.
/// </summary>
/// <param name="DocumentIds">Unique identifiers of the documents that are part of the evidence.</param>
/// <param name="Description">Textual description of evidence.</param>
public record AddEvidenceRequest(string[] DocumentIds, string Description);