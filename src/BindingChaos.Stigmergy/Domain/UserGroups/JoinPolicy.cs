using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Defines the policy that governs how participants may join a <see cref="UserGroup"/>.
/// </summary>
public sealed class JoinPolicy : Enumeration<JoinPolicy>
{
    /// <summary>
    /// Any participant may join without restriction.
    /// </summary>
    public static readonly JoinPolicy Open = new(1, nameof(Open));

    /// <summary>
    /// Participants may only join if invited by an existing member.
    /// </summary>
    public static readonly JoinPolicy InviteOnly = new(2, nameof(InviteOnly));

    /// <summary>
    /// Join requests require explicit approval from the group.
    /// </summary>
    public static readonly JoinPolicy Approval = new(3, nameof(Approval));

    /// <summary>
    /// Initializes a new instance of the <see cref="JoinPolicy"/> class.
    /// </summary>
    /// <param name="id">The numeric identifier for this policy.</param>
    /// <param name="name">The name of this policy.</param>
    private JoinPolicy(int id, string name)
        : base(id, name)
    {
    }
}
