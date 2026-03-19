using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Tagging.Domain.Tags;

/// <summary>
/// Defines a repository for managing <see cref="Tag"/> entities, providing methods for data access and manipulation.
/// </summary>
public interface ITagRepository : IRepository<Tag, TagId>
{
}
