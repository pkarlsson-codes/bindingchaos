using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Reopens a lapsed inquiry. Only the original raiser may reopen.</summary>
/// <param name="InquiryId">The inquiry to reopen.</param>
/// <param name="ActorId">The participant reopening; must be the original raiser.</param>
/// <param name="UpdatedBody">Optional updated body text.</param>
public sealed record ReopenProjectInquiry(ProjectInquiryId InquiryId, ParticipantId ActorId, string? UpdatedBody);

/// <summary>Handler for <see cref="ReopenProjectInquiry"/>.</summary>
public static class ReopenProjectInquiryHandler
{
    /// <summary>Handles the command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="inquiryRepository">Repository for loading and staging the inquiry.</param>
    /// <param name="messageContext">Wolverine message context for scheduling the lapse check.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ReopenProjectInquiry command,
        IProjectInquiryRepository inquiryRepository,
        IMessageContext messageContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var inquiry = await inquiryRepository
            .GetByIdOrThrowAsync(command.InquiryId, cancellationToken)
            .ConfigureAwait(false);

        inquiry.Reopen(command.ActorId, command.UpdatedBody);
        inquiryRepository.Stage(inquiry);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        await messageContext.SendAsync(
            new ScheduleInquiryLapse(inquiry.Id.Value),
            new DeliveryOptions { ScheduleDelay = inquiry.LapseWindow })
            .ConfigureAwait(false);
    }
}
