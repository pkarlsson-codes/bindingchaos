using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.Societies.Events;
using Marten.Events.Projections;

namespace BindingChaos.Societies.Infrastructure.Projections;

/// <summary>
/// Marten multi-stream projection that builds <see cref="SocietyMemberView"/> keyed by membership ID.
/// Members are marked inactive (<see cref="SocietyMemberView.IsActive"/> = false) when they leave.
/// </summary>
internal sealed class SocietyMemberViewProjection : MultiStreamProjection<SocietyMemberView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SocietyMemberViewProjection"/> class,
    /// configuring event identity mappings.
    /// </summary>
    public SocietyMemberViewProjection()
    {
        Identities<MemberJoined>(e => [e.MembershipId]);
        Identities<MemberLeft>(e => [e.MembershipId]);
    }

    /// <summary>
    /// Creates a new <see cref="SocietyMemberView"/> from a <see cref="MemberJoined"/> event.
    /// </summary>
    /// <param name="e">The event containing member details.</param>
    /// <returns>A new <see cref="SocietyMemberView"/>.</returns>
    public static SocietyMemberView Create(MemberJoined e)
    {
        return new SocietyMemberView
        {
            Id = e.MembershipId,
            SocietyId = e.AggregateId,
            ParticipantId = e.ParticipantId,
            SocialContractId = e.SocialContractId,
            JoinedAt = e.OccurredAt,
            IsActive = true,
        };
    }

    /// <summary>
    /// Marks the membership as inactive when a <see cref="MemberLeft"/> event is received.
    /// </summary>
    /// <param name="e">The leave event.</param>
    /// <param name="view">The existing membership view to update.</param>
    public static void Apply(MemberLeft e, SocietyMemberView view)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);
        view.IsActive = false;
    }
}
