using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Represents the current status of an idea.
/// </summary>
public sealed class IdeaStatus : Enumeration<IdeaStatus>
{
    /// <summary>
    /// The idea is published and publicly visible.
    /// </summary>
    public static readonly IdeaStatus Published = new(1, nameof(Published));

    /// <summary>
    /// Initializes a new instance of the IdeaStatus class.
    /// </summary>
    /// <param name="id">The numeric identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private IdeaStatus(int id, string name)
        : base(id, name)
    {
    }
}