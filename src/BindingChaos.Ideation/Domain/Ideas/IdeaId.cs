using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Unique identifier for an <see cref="Idea"/>.
/// </summary>
public sealed class IdeaId : EntityId<IdeaId>
{
    private const string Prefix = "idea";

    private IdeaId(string value)
        : base(value, Prefix)
    {
    }
}