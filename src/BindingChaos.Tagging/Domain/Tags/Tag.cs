using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Tagging.Domain.Tags.Events;

namespace BindingChaos.Tagging.Domain.Tags;

/// <summary>
/// Represents a globally-scoped tag entity with lifecycle metadata.
/// </summary>
public sealed class Tag : AggregateRoot<TagId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    private Tag() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class with the specified identifier and preferred label.
    /// </summary>
    /// <param name="id">The unique identifier for the tag.</param>
    /// <param name="preferredLabel">The preferred label for the tag. This value is used to generate a slug.</param>
    /// <param name="createdById">The identifier of the user who created the tag.</param>
    private Tag(TagId id, string preferredLabel, ParticipantId createdById)
    {
        ApplyChange(new TagCreated(id.Value, Version, preferredLabel, TagSlugifier.Slugify(preferredLabel), createdById.Value));
    }

    /// <summary>
    /// Gets a value indicating whether the current tag is deprecated.
    /// </summary>
    public bool IsDeprecated { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="Tag"/> class with the specified preferred label.
    /// </summary>
    /// <param name="preferredLabel">The preferred label for the tag. This value must not be null or empty.</param>
    /// <param name="createdById">The identifier of the user who is creating the tag.</param>
    /// <returns>A new <see cref="Tag"/> instance initialized with the specified label.</returns>
    public static Tag Create(string preferredLabel, ParticipantId createdById)
    {
        ValidateLabel(preferredLabel);
        return new Tag(TagId.Generate(), preferredLabel, createdById);
    }

    /// <summary>
    /// Marks the current tag as deprecated with the specified reason.
    /// </summary>
    /// <param name="reason">The reason for deprecating the tag. This value cannot be null or empty.</param>
    /// <param name="deprecatedById">The identifier of the user who is deprecating the tag.</param>
    public void Deprecate(string reason, string deprecatedById)
    {
        if (IsDeprecated)
        {
            return;
        }

        ApplyChange(new TagDeprecated(Id.Value, Version, reason, deprecatedById));
    }

    /// <summary>
    /// Merges the current tag into the specified target tag, optionally carrying over selected aliases.
    /// </summary>
    /// <param name="target">The target <see cref="TagId"/> into which the current tag will be merged.</param>
    /// <param name="carryOverAliases">A collection of aliases to carry over to the target tag.</param>
    /// <param name="mergedById">The identifier of the user who is performing the merge operation.</param>
    public void MergeInto(TagId target, IEnumerable<string> carryOverAliases, ParticipantId mergedById)
    {
        if (IsDeprecated) { return; }
        ApplyChange(new TagsMerged(Id.Value, Version, target.Value, carryOverAliases.Select(TagSlugifier.Slugify).ToArray(), mergedById.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case TagCreated x: Apply(x); break;
            case TagDeprecated x: Apply(x); break;
            case TagsMerged x: Apply(x); break;
        }
    }

    private static void ValidateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new BusinessRuleViolationException("Label required.");
        }

        if (label.Length > 50)
        {
            throw new BusinessRuleViolationException("Max 50 chars.");
        }
    }

    private void Apply(TagCreated x)
    {
        Id = TagId.Create(x.AggregateId);
    }

    private void Apply(TagsMerged x)
    {
        IsDeprecated = true;
    }

    private void Apply(TagDeprecated x)
    {
        IsDeprecated = true;
    }
}
