using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Resolves a project inquiry. Only the original raiser may resolve.</summary>
/// <param name="InquiryId">The inquiry to resolve.</param>
/// <param name="ActorId">The participant resolving the inquiry; must be the original raiser.</param>
public sealed record ResolveProjectInquiry(ProjectInquiryId InquiryId, ParticipantId ActorId);

/// <summary>Handler for <see cref="ResolveProjectInquiry"/>.</summary>
public static class ResolveProjectInquiryHandler
{
    /// <summary>Handles the command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="inquiryRepository">Repository for loading and staging the inquiry.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ResolveProjectInquiry command,
        IProjectInquiryRepository inquiryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var inquiry = await inquiryRepository
            .GetByIdOrThrowAsync(command.InquiryId, cancellationToken)
            .ConfigureAwait(false);

        inquiry.Resolve(command.ActorId);
        inquiryRepository.Stage(inquiry);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
