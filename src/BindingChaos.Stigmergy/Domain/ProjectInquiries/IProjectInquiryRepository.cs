using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries;

/// <summary>Repository for <see cref="ProjectInquiry"/> aggregates.</summary>
public interface IProjectInquiryRepository : IRepository<ProjectInquiry, ProjectInquiryId>
{
}
