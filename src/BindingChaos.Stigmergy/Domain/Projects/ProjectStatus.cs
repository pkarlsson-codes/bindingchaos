using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Represents the lifecycle status of a project.
/// </summary>
public sealed class ProjectStatus : Enumeration<ProjectStatus>
{
    /// <summary>The project is active and accepting amendments.</summary>
    public static readonly ProjectStatus Active = new(1, nameof(Active));

    /// <summary>The project has been completed.</summary>
    public static readonly ProjectStatus Completed = new(2, nameof(Completed));

    /// <summary>The project has been archived.</summary>
    public static readonly ProjectStatus Archived = new(3, nameof(Archived));

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectStatus"/> class.
    /// </summary>
    /// <param name="id">The numeric identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private ProjectStatus(int id, string name)
        : base(id, name)
    {
    }
}
