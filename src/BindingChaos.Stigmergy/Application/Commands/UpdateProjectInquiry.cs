using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Updates the body of a project inquiry. Resets status to Open.</summary>
/// <param name="InquiryId">The inquiry to update.</param>
/// <param name="ActorId">The participant updating; must be the original raiser.</param>
/// <param name="NewBody">The new body text.</param>
public sealed record UpdateProjectInquiry(ProjectInquiryId InquiryId, ParticipantId ActorId, string NewBody);

/// <summary>Handler for <see cref="UpdateProjectInquiry"/>.</summary>
public static class UpdateProjectInquiryHandler
{
    /// <summary>Handles the command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="inquiryRepository">Repository for loading and staging the inquiry.</param>
    /// <param name="messageContext">Wolverine message context for scheduling the lapse check.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        UpdateProjectInquiry command,
        IProjectInquiryRepository inquiryRepository,
        IMessageContext messageContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var inquiry = await inquiryRepository
            .GetByIdOrThrowAsync(command.InquiryId, cancellationToken)
            .ConfigureAwait(false);

        inquiry.Update(command.ActorId, command.NewBody);
        inquiryRepository.Stage(inquiry);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        await messageContext.SendAsync(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            new DeliveryOptions { ScheduleDelay = inquiry.LapseWindow })
            .ConfigureAwait(false);
    }
}
