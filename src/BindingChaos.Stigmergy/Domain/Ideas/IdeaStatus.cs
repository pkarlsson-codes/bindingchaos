using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Ideas;

/// <summary>
/// Idea status.
/// </summary>
public class IdeaStatus : Enumeration<IdeaStatus>
{
    /// <summary>
    /// Draft.
    /// </summary>
    public static readonly IdeaStatus Draft = new(1, nameof(Draft));

    /// <summary>
    /// Published.
    /// </summary>
    public static readonly IdeaStatus Published = new (2, nameof(Published));

    private IdeaStatus(int id, string name)
        : base(id, name) { }
}