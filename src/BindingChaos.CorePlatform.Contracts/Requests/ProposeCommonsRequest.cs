namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>Request to propose a new commons.</summary>
/// <param name="Name">Name of the commons.</param>
/// <param name="Description">Description of the commons.</param>
public record ProposeCommonsRequest(string Name, string Description);
