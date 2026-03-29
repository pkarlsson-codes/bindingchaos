using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Concerns;

/// <summary>
/// Unique identifier for a <see cref="Concern"/>.
/// </summary>
public class ConcernId : EntityId<ConcernId>
{
    private const string Prefix = "concern";

    private ConcernId(string value)
        : base(value, Prefix)
    {
    }
}