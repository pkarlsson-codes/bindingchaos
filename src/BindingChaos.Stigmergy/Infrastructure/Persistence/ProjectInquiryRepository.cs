using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the ProjectInquiry repository for event sourcing.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal sealed class ProjectInquiryRepository(
    IDocumentSession session,
    ILogger<MartenRepository<ProjectInquiry, ProjectInquiryId>> logger)
    : MartenRepository<ProjectInquiry, ProjectInquiryId>(session, logger), IProjectInquiryRepository
{
}
