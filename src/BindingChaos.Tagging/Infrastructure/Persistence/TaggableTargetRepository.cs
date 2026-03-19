using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Domain.TaggableTargets;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Tagging.Infrastructure.Persistence;

/// <summary>
/// Provides a repository for managing <see cref="TaggableTarget"/> entities,  enabling data access and persistence
/// operations within the context of a Marten document store.
/// </summary>
internal sealed class TaggableTargetRepository(
    IDocumentSession session,
    ILogger<MartenRepository<TaggableTarget, TaggableTargetId>> logger)
    : MartenRepository<TaggableTarget, TaggableTargetId>(session, logger), ITaggableTargetRepository
{
}
