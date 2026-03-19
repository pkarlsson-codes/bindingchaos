using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Tagging.Domain.Tags;

/// <summary>
/// Unique identifier for a <see cref="Tag"/>.
/// </summary>
public sealed class TagId : EntityId<TagId>
{
    private const string Prefix = "tag";

    private TagId(string value)
        : base(value, Prefix)
    {
    }
}