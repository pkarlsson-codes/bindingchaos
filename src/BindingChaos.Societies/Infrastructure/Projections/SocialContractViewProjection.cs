using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.SocialContracts.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Societies.Infrastructure.Projections;

/// <summary>
/// Marten projection that builds <see cref="SocialContractView"/> from SocialContract events.
/// </summary>
internal sealed class SocialContractViewProjection : SingleStreamProjection<SocialContractView, string>
{
    /// <summary>
    /// Creates the initial <see cref="SocialContractView"/> from a <see cref="SocialContractEstablished"/> event.
    /// </summary>
    /// <param name="e">The social contract established event.</param>
    /// <returns>The initialized social contract view.</returns>
    public static SocialContractView Create(IEvent<SocialContractEstablished> e)
    {
        return new SocialContractView
        {
            Id = e.Data.AggregateId,
            SocietyId = e.Data.SocietyId,
            EstablishedAt = e.Data.OccurredAt,
        };
    }
}
