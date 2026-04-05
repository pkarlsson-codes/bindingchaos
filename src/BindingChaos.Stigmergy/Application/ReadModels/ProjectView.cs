namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model for a single project's full detail, including amendment lifecycle state.
/// </summary>
public class ProjectView
{
    /// <summary>Gets or sets the project ID.</summary>
    required public string Id { get; set; }

    /// <summary>Gets or sets the user group that owns the project.</summary>
    required public string UserGroupId { get; set; }

    /// <summary>Gets or sets the project title.</summary>
    required public string Title { get; set; }

    /// <summary>Gets or sets the project description.</summary>
    required public string Description { get; set; }

    /// <summary>Gets or sets when the project was created.</summary>
    required public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets when the project lifecycle was last updated.</summary>
    required public DateTimeOffset LastUpdatedAt { get; set; }

    /// <summary>Gets or sets the amendments proposed against the project.</summary>
    public List<ProjectAmendmentView> Amendments { get; set; } = [];

    /// <summary>
    /// Read model for a single amendment within a project.
    /// </summary>
    public class ProjectAmendmentView
    {
        /// <summary>Gets or sets the amendment ID.</summary>
        required public string Id { get; set; }

        /// <summary>Gets or sets the participant who proposed the amendment.</summary>
        required public string ProposedById { get; set; }

        /// <summary>Gets or sets when the amendment was proposed.</summary>
        required public DateTimeOffset ProposedAt { get; set; }

        /// <summary>Gets or sets the amendment status.</summary>
        required public string Status { get; set; }

        /// <summary>Gets or sets the participant that contested the amendment, when applicable.</summary>
        public string? ContestedById { get; set; }

        /// <summary>Gets or sets when the amendment was contested, when applicable.</summary>
        public DateTimeOffset? ContestedAt { get; set; }

        /// <summary>Gets or sets when the amendment's status last changed.</summary>
        required public DateTimeOffset LastStatusChangedAt { get; set; }
    }
}
