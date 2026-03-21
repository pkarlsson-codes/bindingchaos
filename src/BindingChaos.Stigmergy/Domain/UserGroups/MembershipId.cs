using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Unique identifier for a <see cref="Membership"/>.
/// </summary>
public sealed class MembershipId : EntityId<MembershipId>
{
    private const string Prefix = "membership";

    private MembershipId(string value)
        : base(value, Prefix)
    {
    }
}
