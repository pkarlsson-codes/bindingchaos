using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Domain.Concerns;

/// <summary>
/// A concern.
/// </summary>
public sealed class Concern : AggregateRoot<ConcernId>
{
    private Concern()
    {
    }

    /// <summary>
    /// Raises a concern.
    /// </summary>
    /// <param name="actorId">Id of the actor raising the concern.</param>
    /// <param name="name">The name of the concern.</param>
    /// <param name="signalIds">Ids of <see cref="Signals"/> revealing the concern.</param>
    /// <returns>The raised concern.</returns>
    public static Concern Raise(
        ParticipantId actorId,
        string name,
        IReadOnlyList<SignalId> signalIds)
    {
        var concern = new Concern();
        var concernId = ConcernId.Generate();
        IReadOnlyList<string> signalIdValues = [..signalIds.Select(i => i.Value)];
        concern.ApplyChange(new ConcernRaised(concernId.Value, actorId.Value, name, signalIdValues));
        return concern;
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ConcernRaised e: Apply(e); break;
            default: throw new BusinessRuleViolationException("Invalid domain event");
        }
    }

    private void Apply(ConcernRaised e)
    {
        Id = ConcernId.Create(e.AggregateId);
    }
}