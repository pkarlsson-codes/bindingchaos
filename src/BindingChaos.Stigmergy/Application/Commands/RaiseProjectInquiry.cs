using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using Marten;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Raises a new project inquiry on behalf of an affected society member.</summary>
/// <param name="ProjectId">The project being inquired about.</param>
/// <param name="ActorId">The participant raising the inquiry.</param>
/// <param name="Body">The inquiry body text.</param>
public sealed record RaiseProjectInquiry(
    ProjectId ProjectId,
    ParticipantId ActorId,
    string Body);

/// <summary>Handler for <see cref="RaiseProjectInquiry"/>.</summary>
public static class RaiseProjectInquiryHandler
{
    /// <summary>Handles the command by verifying standing and raising the inquiry.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="projectRepository">Repository for loading projects.</param>
    /// <param name="inquiryRepository">Repository for staging the new inquiry.</param>
    /// <param name="querySession">Marten query session for cross-BC standing checks.</param>
    /// <param name="messageContext">Wolverine message context for scheduling the lapse check.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the newly raised inquiry.</returns>
    public static async Task<ProjectInquiryId> Handle(
        RaiseProjectInquiry command,
        IProjectRepository projectRepository,
        IProjectInquiryRepository inquiryRepository,
        IQuerySession querySession,
        IMessageContext messageContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await projectRepository
            .GetByIdOrThrowAsync(command.ProjectId, cancellationToken)
            .ConfigureAwait(false);

        var ugView = querySession.Query<UserGroupListItemView>()
            .FirstOrDefault(x => x.Id == project.UserGroupId.Value)
            ?? throw new BusinessRuleViolationException("User group view not found.");

        var affectedSocietyIds = querySession.Query<SocietyAffectedByCommonsView>()
            .Where(x => x.CommonsId == ugView.CommonsId)
            .Select(x => x.SocietyId)
            .ToList();

        if (affectedSocietyIds.Count == 0)
        {
            throw new BusinessRuleViolationException("No societies are registered as affected by this project's commons.");
        }

        var membership = querySession.Query<SocietyMemberView>()
            .FirstOrDefault(x =>
                x.ParticipantId == command.ActorId.Value &&
                x.IsActive &&
                affectedSocietyIds.Contains(x.SocietyId))
            ?? throw new BusinessRuleViolationException("Only active members of affected societies can raise inquiries.");

        var contract = querySession.Query<SocialContractView>()
            .Where(x => x.SocietyId == membership.SocietyId)
            .OrderByDescending(x => x.EstablishedAt)
            .FirstOrDefault()
            ?? throw new BusinessRuleViolationException("Society social contract not found.");

        var lapseWindow = TimeSpan.FromTicks(contract.InquiryLapseWindowTicks);

        var inquiry = ProjectInquiry.Raise(
            command.ProjectId,
            command.ActorId,
            membership.SocietyId,
            command.Body,
            lapseWindow);

        inquiryRepository.Stage(inquiry);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        await messageContext.SendAsync(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            new DeliveryOptions { ScheduleDelay = lapseWindow })
            .ConfigureAwait(false);

        return inquiry.Id;
    }
}
