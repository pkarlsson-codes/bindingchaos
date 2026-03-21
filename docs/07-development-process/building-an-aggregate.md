# Building an Aggregate

**ID** — extend `EntityId<T>` with a `private const string Prefix` and a single-string constructor.

```csharp
public class SocietyId : EntityId<SocietyId>
{
    private const string Prefix = "society";
    private SocietyId(string value) : base(value, Prefix) { }
}
```

`Generate()` and `Create(string)` are inherited.

**Value object** — extend `ValueObject`, implement `GetEqualityComponents`.

```csharp
public sealed class EpistemicRules : ValueObject
{
    public EpistemicRules(double requiredVerificationWeight) => RequiredVerificationWeight = requiredVerificationWeight;
    public double RequiredVerificationWeight { get; }
    protected override IEnumerable<object> GetEqualityComponents() { yield return RequiredVerificationWeight; }
}
```

**Enumeration** — extend `Enumeration<T>` with `static readonly` fields and a private constructor. `FromValue`, `FromDisplayName`, `TryFrom*`, and `GetAll` are inherited.

```csharp
public sealed class SocietyRelationshipType : Enumeration<SocietyRelationshipType>
{
    public static readonly SocietyRelationshipType PartOf = new(0, nameof(PartOf));
    public static readonly SocietyRelationshipType Affiliated = new(1, nameof(Affiliated));
    private SocietyRelationshipType(int value, string displayName) : base(value, displayName) { }
}
```

**Events** — `sealed record` extending `DomainEvent`, forwarding `AggregateId` and `Version` to the base.

```csharp
public sealed record SocietyCreated(string AggregateId, long Version, string Name, string CreatedBy)
    : DomainEvent(AggregateId, Version);
```

**Aggregate** — `sealed class` extending `AggregateRoot<TId>`. Private parameterless constructor required; register invariants there. `Id` is set in `Apply`, not the constructor.

```csharp
public sealed class Society : AggregateRoot<SocietyId>
{
    private Society() { AddInvariants(NameMustBeSpecified); }

    public string Name { get; private set; } = default!;

    public static Society Create(ParticipantId createdBy, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleViolationException("Society name must be specified.");
        var society = new Society();
        society.ApplyChange(new SocietyCreated(SocietyId.Generate().Value, society.Version, name, createdBy.Value));
        return society;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new BusinessRuleViolationException("Name must be specified.");
        ApplyChange(new SocietyNameUpdated(Id.Value, Version, newName));
    }

    protected override void ApplyEvent(IDomainEvent domainEvent) => domainEvent switch
    {
        SocietyCreated e => Apply(e),
        SocietyNameUpdated e => Apply(e),
        _ => throw new InvalidOperationException($"Unknown event: {domainEvent?.GetType().Name}")
    };

    private void Apply(SocietyCreated evt) { Id = SocietyId.Create(evt.AggregateId); Name = evt.Name; }
    private void Apply(SocietyNameUpdated evt) => Name = evt.NewName;

    private void NameMustBeSpecified()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvariantViolationException("Society name must be specified.");
    }
}
```

**Child entities** — extend `Entity<TId>`, constructed by the aggregate inside `Apply` methods.

```csharp
public sealed class Membership : Entity<MembershipId>
{
    public Membership(MembershipId id, ParticipantId participantId) { Id = id; ParticipantId = participantId; }
    public ParticipantId ParticipantId { get; }
}
```
