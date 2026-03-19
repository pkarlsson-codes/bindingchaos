using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;

/// <summary>
/// Unique identifier for a <see cref="DiscourseThread"/>.
/// </summary>
public sealed class DiscourseThreadId : EntityId<DiscourseThreadId>
{
    private const string Prefix = "thread";

    private DiscourseThreadId(string value)
        : base(value, Prefix)
    {
    }
}