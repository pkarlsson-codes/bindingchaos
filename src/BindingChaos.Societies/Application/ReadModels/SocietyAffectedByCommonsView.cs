namespace BindingChaos.Societies.Application.ReadModels;

/// <summary>
/// One document per society-commons affectedness declaration.
/// Keyed as "{commonsId}:{societyId}".
/// </summary>
public class SocietyAffectedByCommonsView
{
    /// <summary>Gets or sets the document key: "{commonsId}:{societyId}".</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the commons identifier.</summary>
    public string CommonsId { get; set; } = string.Empty;

    /// <summary>Gets or sets the society identifier.</summary>
    public string SocietyId { get; set; } = string.Empty;

    /// <summary>Gets or sets when the declaration was made.</summary>
    public DateTimeOffset DeclaredAt { get; set; }
}
