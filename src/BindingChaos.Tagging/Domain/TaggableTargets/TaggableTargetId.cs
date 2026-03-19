using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Tagging.Domain.TaggableTargets;

/// <summary>
/// Unique identifier for a <see cref="TaggableTarget"/>.
/// </summary>
public sealed class TaggableTargetId : EntityId<TaggableTargetId>
{
    private const string Prefix = "tagtarget";

    private TaggableTargetId(string value)
        : base(value, Prefix)
    {
    }

    /// <summary>
    /// Gets the original entity ID (e.g. <c>idea-abc123</c>) by stripping the stream prefix.
    /// This is the ID that consuming bounded contexts use to locate their entity.
    /// </summary>
    public string EntityId => Value[(Prefix.Length + 1)..];

    /// <summary>
    /// Creates a <see cref="TaggableTargetId"/> for the given source entity ID, applying the stream prefix.
    /// </summary>
    /// <param name="entityId">The source entity's own ID (e.g. <c>idea-abc123</c>).</param>
    /// <returns>A new <see cref="TaggableTargetId"/> with stream key <c>tagtarget-{entityId}</c>.</returns>
    public static TaggableTargetId ForEntity(string entityId)
        => Create(Prefix + "-" + entityId);
}
