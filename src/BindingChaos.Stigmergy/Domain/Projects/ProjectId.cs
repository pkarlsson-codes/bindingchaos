using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Unique identifier for a <see cref="ProjectId"/>.
/// </summary>
public sealed class ProjectId : EntityId<ProjectId>
{
    private const string Prefix = "project";

    private ProjectId(string value)
        : base(value, Prefix)
    {
    }
}
