using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Handler for scheduled <see cref="ScheduleInquiryLapse"/> messages.</summary>
public static class LapseProjectInquiryHandler
{
    /// <summary>
    /// Handles a scheduled lapse check. Lapses the inquiry if still open or responded;
    /// no-op if it has already been resolved or lapsed.
    /// </summary>
    /// <param name="message">The scheduled lapse message.</param>
    /// <param name="inquiryRepository">Repository for loading and staging the inquiry.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ScheduleInquiryLapse message,
        IProjectInquiryRepository inquiryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var inquiry = await inquiryRepository
            .GetByIdOrThrowAsync(ProjectInquiryId.Create(message.InquiryId), cancellationToken)
            .ConfigureAwait(false);

        var statusBefore = inquiry.Status;
        inquiry.Lapse();

        if (inquiry.Status != statusBefore)
        {
            inquiryRepository.Stage(inquiry);
            await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
