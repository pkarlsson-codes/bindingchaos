using System.Text.Json;

using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies.Events;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Aggregate root representing a society — a self-governing group of participants.
/// </summary>
public sealed class Society : AggregateRoot<SocietyId>
{
    private readonly List<string> _tags = [];

    private readonly List<SocietyRelationship> _relationships = [];

    private readonly List<Membership> _memberships = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    private Society()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the name of the society.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the description of the society.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the participant who created this society.
    /// </summary>
    public ParticipantId CreatedBy { get; private set; }

    /// <summary>
    /// Gets the timestamp when this society was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the optional geographic bounds of this society.
    /// </summary>
    public GeographicArea? GeographicBounds { get; private set; }

    /// <summary>
    /// Gets the optional center coordinates of this society.
    /// </summary>
    public Coordinates? Center { get; private set; }

    /// <summary>
    /// Gets the tags associated with this society.
    /// </summary>
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Gets the relationships this society has with other societies.
    /// </summary>
    public IReadOnlyCollection<SocietyRelationship> Relationships => _relationships.AsReadOnly();

    /// <summary>
    /// Gets the memberships in this society.
    /// </summary>
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();

    /// <summary>
    /// Creates a new society.
    /// </summary>
    /// <param name="createdBy">The participant creating the society.</param>
    /// <param name="name">The name of the society.</param>
    /// <param name="description">The description of the society.</param>
    /// <param name="tags">The initial tags.</param>
    /// <param name="geographicBounds">Optional geographic bounds.</param>
    /// <param name="center">Optional center coordinates.</param>
    /// <returns>A new <see cref="Society"/> instance.</returns>
    public static Society Create(
        ParticipantId createdBy,
        string name,
        string description,
        string[] tags,
        GeographicArea? geographicBounds,
        Coordinates? center)
    {
        ArgumentNullException.ThrowIfNull(createdBy);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleViolationException("Society name must be specified.");
        }

        var society = new Society();
        var id = SocietyId.Generate();

        var boundsJson = geographicBounds is null
            ? null
            : JsonSerializer.Serialize(new { geographicBounds.North, geographicBounds.South, geographicBounds.East, geographicBounds.West });

        var centerJson = center is null
            ? null
            : JsonSerializer.Serialize(new { center.Latitude, center.Longitude });

        society.ApplyChange(new SocietyCreated(
            id.Value,
            society.Version,
            name,
            description,
            createdBy.Value,
            tags,
            boundsJson,
            centerJson));

        return society;
    }

    /// <summary>
    /// Updates the society's description.
    /// </summary>
    /// <param name="newDescription">The new description.</param>
    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new BusinessRuleViolationException("Society description must be specified.");
        }

        ApplyChange(new SocietyDescriptionUpdated(Id.Value, Version, newDescription));
    }

    /// <summary>
    /// Adds a tag to this society.
    /// </summary>
    /// <param name="tag">The tag to add.</param>
    public void AddTag(string tag)
    {
        if (_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessRuleViolationException($"Tag '{tag}' already exists on this society.");
        }

        ApplyChange(new SocietyTagAdded(Id.Value, Version, tag));
    }

    /// <summary>
    /// Removes a tag from this society.
    /// </summary>
    /// <param name="tag">The tag to remove.</param>
    public void RemoveTag(string tag)
    {
        if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessRuleViolationException($"Tag '{tag}' does not exist on this society.");
        }

        ApplyChange(new SocietyTagRemoved(Id.Value, Version, tag));
    }

    /// <summary>
    /// Adds a relationship to another society.
    /// </summary>
    /// <param name="targetSocietyId">The target society.</param>
    /// <param name="relationshipType">The type of relationship.</param>
    public void AddRelationship(SocietyId targetSocietyId, SocietyRelationshipType relationshipType)
    {
        ArgumentNullException.ThrowIfNull(targetSocietyId);

        var existing = _relationships.FirstOrDefault(r =>
            r.TargetSocietyId.Value == targetSocietyId.Value && r.RelationshipType == relationshipType);

        if (existing is not null)
        {
            throw new BusinessRuleViolationException(
                $"A {relationshipType} relationship with society '{targetSocietyId.Value}' already exists.");
        }

        ApplyChange(new SocietyRelationshipAdded(Id.Value, Version, targetSocietyId.Value, relationshipType.DisplayName));
    }

    /// <summary>
    /// Removes a relationship to another society.
    /// </summary>
    /// <param name="targetSocietyId">The target society.</param>
    /// <param name="relationshipType">The type of relationship to remove.</param>
    public void RemoveRelationship(SocietyId targetSocietyId, SocietyRelationshipType relationshipType)
    {
        ArgumentNullException.ThrowIfNull(targetSocietyId);

        var existing = _relationships.FirstOrDefault(r =>
            r.TargetSocietyId.Value == targetSocietyId.Value && r.RelationshipType == relationshipType);

        if (existing is null)
        {
            throw new BusinessRuleViolationException(
                $"No {relationshipType} relationship with society '{targetSocietyId.Value}' found.");
        }

        ApplyChange(new SocietyRelationshipRemoved(Id.Value, Version, targetSocietyId.Value, relationshipType.DisplayName));
    }

    /// <summary>
    /// Adds a member to this society.
    /// </summary>
    /// <param name="participantId">The participant joining.</param>
    /// <param name="socialContractId">The social contract being agreed to.</param>
    /// <returns>The new membership identifier.</returns>
    public MembershipId Join(ParticipantId participantId, SocialContractId socialContractId)
    {
        ArgumentNullException.ThrowIfNull(participantId);
        ArgumentNullException.ThrowIfNull(socialContractId);

        var hasActiveMembership = _memberships.Any(m => m.ParticipantId.Value == participantId.Value && m.IsActive);
        if (hasActiveMembership)
        {
            throw new BusinessRuleViolationException(
                $"Participant '{participantId.Value}' already has an active membership in this society.");
        }

        var membershipId = MembershipId.Generate();
        ApplyChange(new MemberJoined(Id.Value, Version, membershipId.Value, participantId.Value, socialContractId.Value));
        return membershipId;
    }

    /// <summary>
    /// Removes a member from this society.
    /// </summary>
    /// <param name="participantId">The participant leaving.</param>
    public void Leave(ParticipantId participantId)
    {
        ArgumentNullException.ThrowIfNull(participantId);

        var membership = _memberships.FirstOrDefault(m => m.ParticipantId.Value == participantId.Value && m.IsActive);
        if (membership is null)
        {
            throw new BusinessRuleViolationException(
                $"Participant '{participantId.Value}' does not have an active membership in this society.");
        }

        ApplyChange(new MemberLeft(Id.Value, Version, membership.Id.Value, participantId.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SocietyCreated e: Apply(e); break;
            case SocietyDescriptionUpdated e: Apply(e); break;
            case SocietyTagAdded e: Apply(e); break;
            case SocietyTagRemoved e: Apply(e); break;
            case SocietyRelationshipAdded e: Apply(e); break;
            case SocietyRelationshipRemoved e: Apply(e); break;
            case MemberJoined e: Apply(e); break;
            case MemberLeft e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void RegisterInvariants()
    {
        AddInvariants(
            NameMustBeSpecified,
            CreatorMustBeSpecified);
    }

    private void Apply(SocietyCreated evt)
    {
        Id = SocietyId.Create(evt.AggregateId);
        Name = evt.Name;
        Description = evt.Description;
        CreatedBy = new ParticipantId(evt.CreatedBy);
        CreatedAt = evt.OccurredAt;
        _tags.AddRange(evt.Tags);

        if (evt.GeographicBoundsJson is not null)
        {
            var doc = JsonSerializer.Deserialize<JsonElement>(evt.GeographicBoundsJson);
            GeographicBounds = new GeographicArea(
                doc.GetProperty("North").GetDouble(),
                doc.GetProperty("South").GetDouble(),
                doc.GetProperty("East").GetDouble(),
                doc.GetProperty("West").GetDouble());
        }

        if (evt.CenterJson is not null)
        {
            var doc = JsonSerializer.Deserialize<JsonElement>(evt.CenterJson);
            Center = new Coordinates(
                doc.GetProperty("Latitude").GetDouble(),
                doc.GetProperty("Longitude").GetDouble());
        }
    }

    private void Apply(SocietyDescriptionUpdated evt)
    {
        Description = evt.NewDescription;
    }

    private void Apply(SocietyTagAdded evt)
    {
        _tags.Add(evt.Tag);
    }

    private void Apply(SocietyTagRemoved evt)
    {
        _tags.Remove(evt.Tag);
    }

    private void Apply(SocietyRelationshipAdded evt)
    {
        _relationships.Add(new SocietyRelationship(
            SocietyId.Create(evt.TargetSocietyId),
            SocietyRelationshipType.FromDisplayName(evt.RelationshipType)));
    }

    private void Apply(SocietyRelationshipRemoved evt)
    {
        var relationshipType = SocietyRelationshipType.FromDisplayName(evt.RelationshipType);
        var rel = _relationships.FirstOrDefault(r =>
            r.TargetSocietyId.Value == evt.TargetSocietyId && r.RelationshipType == relationshipType);

        if (rel is not null)
        {
            _relationships.Remove(rel);
        }
    }

    private void Apply(MemberJoined evt)
    {
        var membership = new Membership(
            MembershipId.Create(evt.MembershipId),
            new ParticipantId(evt.ParticipantId),
            SocialContractId.Create(evt.SocialContractId),
            evt.OccurredAt);
        _memberships.Add(membership);
    }

    private void Apply(MemberLeft evt)
    {
        var membership = _memberships.FirstOrDefault(m => m.Id.Value == evt.MembershipId);
        if (membership is not null)
        {
            membership.IsActive = false;
        }
    }

    #region Invariants

    private void NameMustBeSpecified()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvariantViolationException("Society name must be specified.");
        }
    }

    private void CreatorMustBeSpecified()
    {
        if (CreatedBy is null)
        {
            throw new InvariantViolationException("Society creator must be specified.");
        }
    }

    #endregion
}
