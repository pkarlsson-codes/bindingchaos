using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons;

/// <summary>Unique identifier for a <see cref="Commons"/>.</summary>
public sealed class CommonsId : EntityId<CommonsId>
{
    private const string Prefix = "commons";

    private CommonsId(string value)
        : base(value, Prefix)
    {
    }
}
