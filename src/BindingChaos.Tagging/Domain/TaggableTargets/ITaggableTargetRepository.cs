using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Tagging.Domain.TaggableTargets;

/// <summary>
/// Defines a repository for managing <see cref="TaggableTarget"/> entities,  providing methods for retrieving, adding,
/// updating, and deleting taggable targets  associated with a specific <see cref="TaggableTargetId"/>.
/// </summary>
public interface ITaggableTargetRepository : IRepository<TaggableTarget, TaggableTargetId>
{
}