using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;

/// <summary>
/// Repository interface for discourse thread persistence operations.
/// </summary>
public interface IDiscourseThreadRepository : IRepository<DiscourseThread, DiscourseThreadId>
{
}