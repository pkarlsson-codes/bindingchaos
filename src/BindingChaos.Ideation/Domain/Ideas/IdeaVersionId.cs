using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Unique identifier for an <see cref="IdeaVersion"/>.
/// </summary>
public sealed class IdeaVersionId : EntityId<IdeaVersionId>
{
    private const string Prefix = "idea-version";

    private IdeaVersionId(string value)
        : base(value, Prefix)
    {
    }
}