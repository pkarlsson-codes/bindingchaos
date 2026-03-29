namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Draft idea request model.
/// </summary>
/// <param name="Title">The title of the idea.</param>
/// <param name="Description">The description of the idea.</param>
public record DraftIdeaRequest(
    string Title,
    string Description);
