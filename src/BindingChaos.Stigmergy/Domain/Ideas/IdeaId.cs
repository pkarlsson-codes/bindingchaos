using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Ideas;

/// <summary>
/// Unique identifier for an <see cref="Idea"/>.
/// </summary>
public class IdeaId : EntityId<IdeaId>
{
    private const string Prefix = "idea";

    private IdeaId(string value)
        : base(value, Prefix)
    {
    }
}