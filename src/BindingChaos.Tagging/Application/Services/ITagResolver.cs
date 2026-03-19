using BindingChaos.SharedKernel.Domain;
using BindingChaos.Tagging.Domain.Tags;

namespace BindingChaos.Tagging.Application.Services;

/// <summary>
/// Resolves existing tags by label or creates new ones.
/// </summary>
public interface ITagResolver
{
    /// <summary>
    /// Resolves existing tags or creates new tags for the specified labels.
    /// </summary>
    /// <param name="inputLabels">The labels to resolve or create.</param>
    /// <param name="originatorId">The participant initiating the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of <see cref="TagId"/> values for the resolved or created tags.</returns>
    Task<TagId[]> ResolveOrCreate(
        string[] inputLabels,
        ParticipantId originatorId,
        CancellationToken cancellationToken);
}
