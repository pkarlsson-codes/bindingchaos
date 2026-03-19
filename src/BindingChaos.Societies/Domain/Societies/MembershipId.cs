using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Unique identifier for a <see cref="Membership"/> entity.
/// </summary>
public class MembershipId : EntityId<MembershipId>
{
    private const string Prefix = "membership";

    private MembershipId(string value)
        : base(value, Prefix)
    {
    }
}
