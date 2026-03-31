using System.Security.Cryptography.X509Certificates;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for <see cref="ConcernsListItemView"/>.
/// </summary>
internal sealed class ConcernsListItemViewProjection
: SingleStreamProjection<ConcernsListItemView, string>
{
    /// <summary>
    /// Creates a new <see cref="ConcernsListItemView"/> from a <see cref="ConcernRaised"/> event.
    /// </summary>
    /// <param name="e">The <see cref="ConcernRaised"/> event to create the read model from.</param>
    /// <returns>A new <see cref="ConcernsListItemView"/> representing the concern raised event.</returns>
    public static ConcernsListItemView Create(ConcernRaised e) => new()
    {
        Id = e.AggregateId,
        RaisedById = e.ActorId,
        Name = e.Name,
        Tags = [.. e.Tags],
        SignalIds = [.. e.SignalIds],
    };
}