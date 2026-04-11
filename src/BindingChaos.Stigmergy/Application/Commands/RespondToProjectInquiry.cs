using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Records the user group's response to a project inquiry.</summary>
/// <param name="InquiryId">The inquiry to respond to.</param>
/// <param name="ProjectId">The project the inquiry is about.</param>
/// <param name="ActorId">The user group member providing the response.</param>
/// <param name="Response">The response text.</param>
public sealed record RespondToProjectInquiry(
    ProjectInquiryId InquiryId,
    ProjectId ProjectId,
    ParticipantId ActorId,
    string Response);

/// <summary>Handler for <see cref="RespondToProjectInquiry"/>.</summary>
public static class RespondToProjectInquiryHandler
{
    /// <summary>Handles the command by verifying membership and recording the response.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="inquiryRepository">Repository for loading and staging the inquiry.</param>
    /// <param name="projectRepository">Repository for loading the project.</param>
    /// <param name="userGroupRepository">Repository for loading the user group.</param>
    /// <param name="messageContext">Wolverine message context for scheduling the lapse check.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        RespondToProjectInquiry command,
        IProjectInquiryRepository inquiryRepository,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IMessageContext messageContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var inquiry = await inquiryRepository
            .GetByIdOrThrowAsync(command.InquiryId, cancellationToken)
            .ConfigureAwait(false);

        var project = await projectRepository
            .GetByIdOrThrowAsync(command.ProjectId, cancellationToken)
            .ConfigureAwait(false);

        var userGroup = await userGroupRepository
            .GetByIdOrThrowAsync(project.UserGroupId, cancellationToken)
            .ConfigureAwait(false);

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ActorId))
        {
            throw new BusinessRuleViolationException("Only user group members can respond to inquiries.");
        }

        inquiry.Respond(command.Response);
        inquiryRepository.Stage(inquiry);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        await messageContext.SendAsync(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            new DeliveryOptions { ScheduleDelay = inquiry.LapseWindow })
            .ConfigureAwait(false);
    }
}
