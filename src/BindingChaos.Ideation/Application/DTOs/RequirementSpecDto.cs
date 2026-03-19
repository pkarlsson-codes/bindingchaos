namespace BindingChaos.Ideation.Application.DTOs;

/// <summary>
/// DTO for the requirement specification.
/// </summary>
public record RequirementSpecDto(string Label, double Quantity, string Unit, string Type);