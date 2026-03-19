using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Societies.Domain.SocialContracts;

/// <summary>
/// Unique identifier for a <see cref="SocialContract"/>.
/// </summary>
public class SocialContractId : EntityId<SocialContractId>
{
    private const string Prefix = "social-contract";

    private SocialContractId(string value)
        : base(value, Prefix)
    {
    }
}
