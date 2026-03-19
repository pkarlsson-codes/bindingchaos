using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>Pure structural mapping from amendment read models to API response contracts.</summary>
internal static class AmendmentMapper
{
    /// <summary>Maps a <see cref="AmendmentsListItemView"/> to a <see cref="AmendmentsListItemResponse"/>.</summary>
    /// <param name="amendment">The list-item read model to map.</param>
    /// <param name="currentParticipantId">The current participant, used to determine authorship and vote status.</param>
    /// <param name="authorPseudonym">The pre-resolved pseudonym for the amendment author.</param>
    /// <returns>The mapped <see cref="AmendmentsListItemResponse"/>.</returns>
    internal static AmendmentsListItemResponse ToAmendmentsListItemResponse(
        AmendmentsListItemView amendment,
        ParticipantId currentParticipantId,
        string authorPseudonym)
    {
        return new AmendmentsListItemResponse
        {
            Id = amendment.Id,
            IdeaId = amendment.IdeaId,
            AuthoredByCurrentUser = amendment.AuthorId == currentParticipantId.Value,
            AuthorPseudonym = authorPseudonym,
            AmendmentTitle = amendment.AmendmentTitle,
            AmendmentDescription = amendment.AmendmentDescription,
            Status = AmendmentStatus.FromValue(amendment.Status).DisplayName,
            OpponentCount = amendment.OpponentIds.Count,
            OpposedByCurrentUser = amendment.OpponentIds.Contains(currentParticipantId.Value),
            SupporterCount = amendment.SupporterIds.Count,
            SupportedByCurrentUser = amendment.SupporterIds.Contains(currentParticipantId.Value),
            CreatedAt = amendment.CreatedAt,
        };
    }

    /// <summary>Maps a <see cref="AmendmentDetailView"/> to a <see cref="AmendmentResponse"/> using a pre-resolved pseudonym.</summary>
    /// <param name="amendment">The amendment detail read model to map.</param>
    /// <param name="currentParticipantId">The current participant, used to determine authorship and vote status.</param>
    /// <param name="creatorPseudonym">The pre-resolved pseudonym for the amendment creator.</param>
    /// <returns>The mapped <see cref="AmendmentResponse"/>.</returns>
    internal static AmendmentResponse ToAmendmentResponse(
        AmendmentDetailView amendment,
        ParticipantId currentParticipantId,
        string creatorPseudonym)
    {
        var status = AmendmentStatus.FromValue(amendment.Status);
        var isOpen = status == AmendmentStatus.Open;
        var isResolved = status == AmendmentStatus.Approved || status == AmendmentStatus.Rejected || status == AmendmentStatus.Withdrawn;

        return new AmendmentResponse
        {
            Id = amendment.Id,
            TargetIdeaId = amendment.IdeaId,
            TargetVersionNumber = amendment.TargetVersionNumber,
            CreatorPseudonym = creatorPseudonym,
            CreatedByCurrentUser = amendment.CreatorId == currentParticipantId.Value,
            Status = status.DisplayName,
            ProposedTitle = amendment.ProposedTitle,
            ProposedBody = amendment.ProposedBody,
            AmendmentTitle = amendment.AmendmentTitle,
            AmendmentDescription = amendment.AmendmentDescription,
            CreatedAt = amendment.CreatedAt,
            AcceptedAt = amendment.AcceptedAt,
            RejectedAt = amendment.RejectedAt,
            IsOpen = isOpen,
            IsResolved = isResolved,
            SupportedByCurrentUser = amendment.SupporterIds.Contains(currentParticipantId.Value),
            OpposedByCurrentUser = amendment.OpponentIds.Contains(currentParticipantId.Value),
        };
    }

    /// <summary>Maps a single <see cref="AmendmentSupporterView"/> to an <see cref="AmendmentSupporterResponse"/> using pre-resolved pseudonyms.</summary>
    /// <param name="supporter">The supporter to map.</param>
    /// <param name="pseudonyms">Pre-resolved pseudonym map (userId → pseudonym) scoped to the idea.</param>
    /// <returns>The mapped <see cref="AmendmentSupporterResponse"/>.</returns>
    internal static AmendmentSupporterResponse ToAmendmentSupporterResponse(
        AmendmentSupporterView supporter,
        IReadOnlyDictionary<string, string> pseudonyms)
    {
        return new AmendmentSupporterResponse
        {
            Id = supporter.Id,
            Pseudonym = pseudonyms[supporter.ParticipantId],
            Reason = supporter.Reason,
            SupportedAt = supporter.SupportedAt.ToString("O"),
        };
    }

    /// <summary>Maps a single <see cref="AmendmentOpponentView"/> to an <see cref="AmendmentOpponentResponse"/> using pre-resolved pseudonyms.</summary>
    /// <param name="opponent">The opponent to map.</param>
    /// <param name="pseudonyms">Pre-resolved pseudonym map (userId → pseudonym) scoped to the idea.</param>
    /// <returns>The mapped <see cref="AmendmentOpponentResponse"/>.</returns>
    internal static AmendmentOpponentResponse ToAmendmentOpponentResponse(
        AmendmentOpponentView opponent,
        IReadOnlyDictionary<string, string> pseudonyms)
    {
        return new AmendmentOpponentResponse
        {
            Id = opponent.Id,
            Pseudonym = pseudonyms[opponent.ParticipantId],
            Reason = opponent.Reason,
            OpposedAt = opponent.OpposedAt.ToString("O"),
        };
    }

    /// <summary>Maps a <see cref="AmendmentTrendView"/> to a <see cref="AmendmentTrendResponse"/>.</summary>
    /// <param name="trend">The trend data to map.</param>
    /// <returns>The mapped <see cref="AmendmentTrendResponse"/>.</returns>
    internal static AmendmentTrendResponse ToAmendmentTrendResponse(AmendmentTrendView trend)
    {
        var dataPoints = trend.DataPoints.Select(point => new TrendPointResponse
        {
            Date = point.Date.ToString("O"),
            EventType = point.VoteType,
        }).ToList();

        return new AmendmentTrendResponse
        {
            AmendmentId = trend.AmendmentId,
            DataPoints = dataPoints,
        };
    }
}
