using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons;

/// <summary>Lifecycle status of a <see cref="Commons"/>.</summary>
public sealed class CommonsStatus : Enumeration<CommonsStatus>
{
    /// <summary>Created but no UserGroup has formed yet.</summary>
    public static readonly CommonsStatus Proposed = new(1, nameof(Proposed));

    /// <summary>At least one UserGroup is actively governing this commons.</summary>
    public static readonly CommonsStatus Active = new(2, nameof(Active));

    private CommonsStatus(int id, string name)
        : base(id, name)
    {
    }
}