using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Domain.Tags;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Tagging.Infrastructure.Persistence;

/// <summary>
/// Provides a repository for managing <see cref="Tag"/> entities, including database interactions and transactional
/// messaging support.
/// </summary>
internal sealed class TagRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Tag, TagId>> logger)
    : MartenRepository<Tag, TagId>(session, logger), ITagRepository
{
}