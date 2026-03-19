namespace BindingChaos.Societies.Application.ReadModels;

/// <summary>
/// Read model for a society relationship embedded in <see cref="SocietyView"/>.
/// </summary>
public class SocietyRelationshipView
{
    /// <summary>
    /// Gets or sets the target society ID.
    /// </summary>
    public string TargetSocietyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship type name.
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;
}
