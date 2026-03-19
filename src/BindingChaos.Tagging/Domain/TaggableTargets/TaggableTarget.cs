using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;
using BindingChaos.Tagging.Domain.Tags;

namespace BindingChaos.Tagging.Domain.TaggableTargets;

/// <summary>
/// Represents a session for managing tags associated with a specific target reference.
/// </summary>
public sealed class TaggableTarget : AggregateRoot<TaggableTargetId>
{
    private readonly List<TagId> _tagIds = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TaggableTarget"/> class with the specified target reference.
    /// </summary>
    /// <param name="id">The target reference associated with this tagging session.</param>
    public TaggableTarget(TaggableTargetId id)
    {
        ApplyChange(new TaggableTargetCreated(id.Value, 0));
    }

    /// <summary>
    /// Assigns the specified tags to the participant for tracking and auditing purposes.
    /// </summary>
    /// <param name="tagIds">An array of tag identifiers to assign to the participant. Each element must be a valid, non-null tag identifier.</param>
    /// <param name="by">The identifier of the participant performing the tag assignment. Cannot be null.</param>
    public void AssignTags(TagId[] tagIds, ParticipantId by)
    {
        ArgumentNullException.ThrowIfNull(tagIds);
        ArgumentNullException.ThrowIfNull(by.Value);

        if (tagIds.Length == 0)
        {
            throw new BusinessRuleViolationException("At least one tag must be provided when assigning tags.");
        }

        var existingTagValues = _tagIds
            .Select(i => i.Value)
            .ToHashSet(StringComparer.Ordinal);

        var tagsToAdd = tagIds
            .Select(i => i.Value)
            .Distinct(StringComparer.Ordinal)
            .Where(v => !existingTagValues.Contains(v))
            .ToArray();

        if (tagsToAdd.Length == 0)
        {
            return;
        }

        ApplyChange(new TagsAssigned(Id.Value, Version, tagsToAdd, by.Value));
    }

    /// <summary>
    /// Removes the specified tags from the participant.
    /// </summary>
    /// <param name="tagIds">An array of tag identifiers to remove from the participant. Each element must correspond to a tag currently
    /// associated with the participant. Cannot be null or empty.</param>
    /// <param name="by">The identifier of the participant performing the removal. Used for auditing purposes. Cannot be null.</param>
    public void RemoveTags(TagId[] tagIds, ParticipantId by)
    {
        ArgumentNullException.ThrowIfNull(tagIds);
        ArgumentNullException.ThrowIfNull(by.Value);

        if (tagIds.Length == 0)
        {
            throw new BusinessRuleViolationException("At least one tag must be provided when removing tags.");
        }

        var existingTagValues = _tagIds
            .Select(i => i.Value)
            .ToHashSet(StringComparer.Ordinal);

        var tagsToRemove = tagIds
            .Select(i => i.Value)
            .Distinct(StringComparer.Ordinal)
            .Where(existingTagValues.Contains)
            .ToArray();

        if (tagsToRemove.Length == 0)
        {
            return;
        }

        ApplyChange(new TagsRemoved(Id.Value, Version, tagsToRemove, by.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case TagsAssigned e: Apply(e); break;
            case TagsRemoved e: Apply(e); break;
            case TaggableTargetCreated e: Apply(e); break;
        }
    }

    private void Apply(TaggableTargetCreated e)
    {
        Id = TaggableTargetId.Create(e.AggregateId);
    }

    private void Apply(TagsAssigned e)
    {
        var existingTagValues = _tagIds
            .Select(i => i.Value)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var tagIdValue in e.TagIds)
        {
            if (existingTagValues.Add(tagIdValue))
            {
                _tagIds.Add(TagId.Create(tagIdValue));
            }
        }
    }

    private void Apply(TagsRemoved e)
    {
        var removedTagValues = e.TagIds.ToHashSet(StringComparer.Ordinal);
        _tagIds.RemoveAll(t => removedTagValues.Contains(t.Value));
    }
}
